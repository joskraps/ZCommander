using System;
using System.Collections.Generic;
using ZCommander.Core.Configuration;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Core.Models;
using ZCommander.Core.Tasks;
using ZCommander.Models;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Tasks
{
    [Serializable()]
    public class SQLTask : BaseSQLTask,ITask
    { 
        public TaskType Type
        {
            get
            {
                return TaskType.Sql;
            }
        }

        public SQLTask(IConfig config)
        {
            base._config = config;
            base.QueryList = new List<SQLStatement>();
            base.Replacements = new List<ReplacementValue>();
        }

        public void Prepare(ref Log returnLog)
        {
            returnLog.Type = LogType.Message;
            returnLog.EndTime = DateTime.Now;
            returnLog.Message = "Prepare successful";
        }
    }
}
