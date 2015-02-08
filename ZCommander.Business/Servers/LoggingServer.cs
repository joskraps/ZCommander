using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZCommander.Core.Configuration;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;

namespace ZCommander.Business.Servers
{
    public class LoggingServer : IObserver<Log>
    {
        private IConfig _config;
        private bool _validTargerServer = false;
        private TextWriterTraceListener tr = null;
        private TraceSource ts= new TraceSource("ZCommander");

        public LoggingServer(IConfig config)
        {
            _config = config;

            if (_config.LoggingEnabled)
            {
                if (_config.LoggingTracePath != "")
                {
                    tr = new TextWriterTraceListener(_config.LoggingTracePath);
                    ts.Listeners.Add(tr);
                }

                if (_config.LoggingConnectionString != "")
                {
                    try
                    {
                        SQLDataExecutor.ExecuteStatement(_config.LoggingConnectionString, "select count(*) from sys.databases", 5);
                        _validTargerServer = true;
                    }
                    catch (Exception ex)
                    {
                        _validTargerServer = false;
                        throw;
                    }

                }
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Log value)
        {
            if (_config.LoggingEnabled)
            {
                lock (this)
                {
                if (tr != null)
                {
                    tr.WriteLine(value.GetBaseMessage());
                    ts.Flush();
                }

                if (_validTargerServer)
                {
                    SQLDataExecutor.ExecuteStatement(_config.LoggingConnectionString, "dbo.Insert_Log ", value.GetSQLParameters());
                }
                }

            }
        }

        public void OnNext(List<Log> value)
        {
            if (_config.LoggingEnabled)
            {
                foreach (Log log in value)
                {
                    if (tr != null)
                    {
                        tr.WriteLine(log.GetBaseMessage());
                        ts.Flush();
                    }

                    if (_validTargerServer)
                    {
                        SQLDataExecutor.ExecuteStatement(_config.LoggingConnectionString, "dbo.Insert_Log ", log.GetSQLParameters());
                    }
                }

            }
        }

    }
}
