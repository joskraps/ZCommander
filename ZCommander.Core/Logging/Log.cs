using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ZCommander.Shared.Enums;

namespace ZCommander.Core.Logging
{
    public class Log
    {
        public string Source { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public LogType Type { get; set; }
        public string Statement { get; set; }

        public Log()
        {
            Successful = true;
            StartTime = DateTime.Now;
        }
        public Log(LogType type) : this()
        {
            this.Type = type;
        }

        public Log(LogType type, string source) : this(type)
        {
            this.Source = source;
        }

        public string GetBaseMessage()
        {
            return string.Format("Source:{0} Start:{1} End:{2} Successful:{3} Message:{4}", Source, StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), EndTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), Successful, Message);
        }

        public List<SqlParameter> GetSQLParameters()
        {
            List<SqlParameter> returnList = new List<SqlParameter>();

            returnList.Add(new SqlParameter("Source", Source));
            returnList.Add(new SqlParameter("StartTime", StartTime));
            returnList.Add(new SqlParameter("EndTime", EndTime));
            returnList.Add(new SqlParameter("Successful", Successful));
            returnList.Add(new SqlParameter("Message", Message));
            returnList.Add(new SqlParameter("Type", Type));
            returnList.Add(new SqlParameter("Statement", Statement));

            return returnList;
        }
    }
}
