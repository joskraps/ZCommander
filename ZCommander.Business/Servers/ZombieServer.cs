using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ZCommander.Business.Factories;
using ZCommander.Core.Configuration;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Core.Models;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Servers
{


    public class ZombieServer : IObservable<Log>
    {
        private IConfig _config;
        private DataManager _dm;
        private ConcurrentBag<IObserver<Log>> observers;

        public ZombieServer()
        {
            observers = new ConcurrentBag<IObserver<Log>>();
        }

        public void Start(IConfig config)
        {
            Start(config, config.Zombies, config.Factories, config.Assemblies);
        }

        public void Start(IConfig config, List<Zombie> zombieList, Dictionary<string, IDataFactory> factoryList, Dictionary<string, ExternalAssembly> assmeblyList)
        {
            Log returnLog = new Log() { Type = LogType.Message, Source = "Zombie Server" };

            _dm = new DataManager(config);
            _config = config;

            foreach (string key in factoryList.Keys)
            {
                SetupFactory(factoryList[key]);
            }

            foreach (string key in assmeblyList.Keys)
            {
                SetupAssembly(assmeblyList[key]);
            }

            foreach (Zombie z in zombieList)
            {
                for (int i = 0; i < z.Multiplier; i++)
                {
                    Zombie clonedZombie = z.Copy<Zombie>();
                    clonedZombie.Name += "-" + i;
                    SetupZombie(clonedZombie);
                }
            }

            returnLog.Successful = true;
            returnLog.EndTime = DateTime.Now;
            returnLog.Message = "Zombie server started!";

            Parallel.ForEach(observers, observer =>
            {
                observer.OnNext(returnLog);
            });

        }

        private void SetupZombie(Zombie z)
        {
            List<Log> prepareLogs = new List<Log>();

            z.Tasks.ForEach(x =>
            {
                var name = z.Name + " - " + x.Name;
                Log zSetupLog = new Log() { Type = LogType.Message, Source = name };
                x.Prepare(ref zSetupLog);
                prepareLogs.Add(zSetupLog);
            });

            if (!prepareLogs.Exists(x => x.Successful = false))
            {
                Timer zTimer = new Timer(z.Frequency * 1000);
                zTimer.Elapsed += (sender, e) => ExecuteZombie(sender, e, z);
                zTimer.AutoReset = true;
                zTimer.Enabled = true;
            }

            Parallel.ForEach(observers, observer =>
                {
                    foreach (Log logger in prepareLogs)
                    {
                        observer.OnNext(logger);
                    }
                });
            //ExecuteZombie(null, null, z);
        }

        private void SetupFactory(IDataFactory factory)
        {
            Log returnLog = new Log() { Type = LogType.Message, Source = factory.Name };
            try
            {
                switch (factory.Type)
                {
                    case DataFactoryType.SQL:
                        SQLDataFactory sqlFac = (SQLDataFactory)factory;
                        if (sqlFac.ConnectionString.ToLower() == "{parent}") { sqlFac.ConnectionString = this._config.SourceConnectionString; }
                        factory.Load(ref returnLog);


                        _dm.DataFactories.TryAdd(factory.Name, factory);

                        if (factory.RefreshValues)
                        {
                            Timer facTimer = new Timer(factory.RefreshRate * 1000);
                            facTimer.Elapsed += (sender, e) => RefreshFactory(sender, e, factory);
                            facTimer.AutoReset = true;
                            facTimer.Enabled = false;
                            facTimer.Enabled = true;
                        }
                        break;
                    case DataFactoryType.JSON:
                        JSONDataFactory jsonFac = (JSONDataFactory)factory;

                        factory.Load(ref returnLog);


                        _dm.DataFactories.TryAdd(factory.Name, factory);

                        if (factory.RefreshValues)
                        {
                            Timer facTimer = new Timer(factory.RefreshRate * 1000);
                            facTimer.Elapsed += (sender, e) => RefreshFactory(sender, e, factory);
                            facTimer.AutoReset = true;
                            facTimer.Enabled = false;
                            facTimer.Enabled = true;
                        }
                        break;

                }
                returnLog.EndTime = DateTime.Now;
                returnLog.Message = "Factory added successfully";
            }
            catch (Exception ex)
            {
                returnLog.Successful = false;
                returnLog.EndTime = DateTime.Now;
                returnLog.Message = "Factory add failed - " + ex.Message;
            }
            Parallel.ForEach(observers, observer =>
            {
                observer.OnNext(returnLog);
            });

        }

        private void SetupAssembly(ExternalAssembly ea)
        {
            Log assemblyLog = new Log(LogType.Message, ea.Name);

            try
            {
                _dm.Assemblies.TryAdd(ea.Name, ea);
                assemblyLog.EndTime = DateTime.Now;
                assemblyLog.Message = "Assembly added successfully";
            }
            catch (Exception ex)
            {
                assemblyLog.Successful = false;
                assemblyLog.EndTime = DateTime.Now;
                assemblyLog.Message = "Assembly add failed - " + ex.Message;
            }


            Parallel.ForEach(observers, observer =>
            {
                observer.OnNext(assemblyLog);
            });
        }

        private void RefreshFactory(object source, ElapsedEventArgs e, IDataFactory f)
        {
            Log returnLog = new Log() { Type = LogType.Message, Source = f.Name };
            f.Refresh(ref returnLog);
            Parallel.ForEach(observers, observer =>
            {
                observer.OnNext(returnLog);
            });
        }

        private void ExecuteZombie(object source, ElapsedEventArgs e, Zombie z)
        {
            Log returnLog = new Log() { Type = LogType.TaskData };
            Task<List<Log>> task = Task<List<Log>>.Factory.StartNew(() => z.Execute(_dm,returnLog));
            task.Wait();
            Parallel.ForEach(observers, observer =>
            {
                foreach (Log logger in task.Result)
                {
                    observer.OnNext(logger);
                }                
            });
        }

        public IDisposable Subscribe(IObserver<Log> observer)
        {
            // Check whether observer is already registered. If not, add it 
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<Log>(observers, observer);
        }
    }

    internal class Unsubscriber<Log> : IDisposable
    {
        private ConcurrentBag<IObserver<Log>> _observers;
        private IObserver<Log> _observer;

        internal Unsubscriber(ConcurrentBag<IObserver<Log>> observers, IObserver<Log> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.TryTake(out _observer);
        }
    }
}
