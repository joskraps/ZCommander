using Microsoft.SqlServer.Management.Trace;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public class SQLTraceTask : BaseSQLTask, ITask
    {

        public string TraceSource { get; set; }
        public string ConnectionString { get; set; }
        public List<SQLEvent> EventList;
        public TaskType Type
        {
            get
            {
                return TaskType.SqlTrace;
            }
        }

        public SQLTraceTask(IConfig config)
        {
            base._config = config;
            base.QueryList = new List<SQLStatement>();
            base.Replacements = new List<ReplacementValue>();
            EventList = new List<SQLEvent>();
        }

        public void Prepare(ref Log returnLog)
        {
            try
            {
                TraceFile source = new TraceFile();
                source.InitializeAsReader(TraceSource);
                while (source.Read())
                {

                    foreach (SQLEvent sqlEvent in EventList)
                    {
                        if (source.GetString(source.GetOrdinal("EventClass")) == sqlEvent.EventName)
                        {
                            SQLStatement newStatement = new SQLStatement();
                            newStatement.Statement = source.GetString(source.GetOrdinal("TextData"));
                            newStatement.ConnectionString = ConnectionString == "{PARENT}" ? _config.SourceConnectionString : ConnectionString;
                            QueryList.Add(newStatement);
                        }
                    }
                }
                returnLog.EndTime = DateTime.Now;
                returnLog.Message = "Prepare successful.";
            }
            catch (Exception ex)
            {
                returnLog.EndTime = DateTime.Now;
                returnLog.Successful = false;
                returnLog.Message = "Prepare failed - " + ex.Message;
            }
        }
    }
}
