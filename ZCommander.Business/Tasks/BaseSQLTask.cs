using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZCommander.Core.Configuration;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Core.Models;
using ZCommander.Core.Tasks;
using ZCommander.Models;
using ZCommander.Output;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Tasks
{
    public abstract class BaseSQLTask
    {
        public IConfig _config;
        public int Sequence { get; set; }
        public string Name { get; set; }
        public List<SQLStatement> QueryList;
        public List<ReplacementValue> Replacements;

        public List<Log> Execute(DataManager dm, Dictionary<string, ITaskVariable> taskVariables, Log returnLog)
        {
            var tempQueryList = QueryList.Copy();
            List<Log> returnLogs = new List<Log>();
            foreach (ReplacementValue strv in Replacements)
            {
                var temp = dm.GetValueFromMacro(strv.Value, taskVariables);
                for (int i = 0; i < tempQueryList.Count; i++)
                {
                    tempQueryList[i].Statement = dm.GetValueFromMacro(tempQueryList[i].Statement, taskVariables);
                    if (strv.Global)
                    {
                        tempQueryList[i].Statement = Regex.Replace(tempQueryList[i].Statement, strv.Pattern, temp);
                    }
                    else
                    {
                        tempQueryList[i].Statement = dm.GetValueFromMacro(tempQueryList[i].Statement, strv, taskVariables);
                    }
                }
            }

            for (int i = 0; i < tempQueryList.Count; i++)
            {
                Log sqlLog = new Log() { Type = returnLog.Type, Source = returnLog.Source };
                try
                {
                    sqlLog.StartTime = DateTime.Now;
                    sqlLog.Source += "-" + i;
                    sqlLog.Statement = dm.GetValueFromMacro(tempQueryList[i].Statement, taskVariables);

                    if (tempQueryList[i].OutputType == SqlStatementOutputType.None)
                    {
                        SQLDataExecutor.ExecuteStatement(tempQueryList[i].ConnectionString, sqlLog.Statement);
                    }
                    else
                    {
                        object output = null;
                        
                        if (tempQueryList[i].OutputType == SqlStatementOutputType.Scalar)
                        {
                            output = SQLDataExecutor.ExecuteScalar(tempQueryList[i].ConnectionString, sqlLog.Statement);
                        }

                        if (tempQueryList[i].OutputList.Count > 0 && output != null)
                        {
                            dm.SetOutputValues(output, tempQueryList[i].OutputList, ref taskVariables);
                        }
                    }


                    sqlLog.Message = "Completed successfully";
                    sqlLog.EndTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    sqlLog.EndTime = DateTime.Now;
                    sqlLog.Successful = false;
                    sqlLog.Message = ex.Message;
                }

                returnLogs.Add(sqlLog);
            }


            return returnLogs;
        }

    }
}
