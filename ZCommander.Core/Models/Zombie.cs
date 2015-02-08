using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Core.Tasks;
using ZCommander.Models;


namespace ZCommander.Core.Models
{
    [Serializable()]
    public class Zombie
    {
        public string Name { get; set; }
        public int Multiplier { get; set; }
        public int Frequency { get; set; }
        public List<ITask> Tasks { get; set; }

        private Dictionary<string, ITaskVariable> _TaskVariables;

        public Dictionary<string, ITaskVariable> TaskVariables { 
            get { return _TaskVariables; } 
            set {
                _TaskVariables = value;
                HasTaskVariables = true;
            } 
        }
        private bool HasTaskVariables = false;

        public Zombie()
        {
            Tasks = new List<ITask>();
            TaskVariables = new Dictionary<string, ITaskVariable>();
        }

        public List<Log> Execute(DataManager dm, Log returnLog)
        {
            returnLog.Source = Name;
            List<Log> returnList = new List<Log>();

            if (HasTaskVariables)
            {
                PrepareLocalVariables(dm);
            }

            foreach (ITask task in Tasks)
            {
                returnList.AddRange(task.Execute(dm, TaskVariables, returnLog));
            }

            if (HasTaskVariables)
            {
                ResetLocalVariables();
            }

            return returnList;
        }

        private void PrepareLocalVariables(DataManager dm)
        {
            foreach (string key in TaskVariables.Keys)
            {
                TaskVariables[key].Load(dm);
            }
        }

        private void ResetLocalVariables()
        {
            foreach (string key in TaskVariables.Keys)
            {
                if (TaskVariables[key].Reset)
                {
                    TaskVariables[key].Value = TaskVariables[key].OriginalValue;
                }
            }
        }
    }
}
