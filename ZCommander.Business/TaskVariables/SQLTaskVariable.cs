using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Core.Data;
using ZCommander.Core.Managers;
using ZCommander.Models;
using ZCommander.Shared.Enums;
using ZCommander.Core.Tasks;

namespace ZCommander.Business.TaskVariables
{
    public class SQLTaskVariable : ITaskVariable
    {
        public string Name { get; set; }
        public bool Reset { get; set; }
        public string OriginalValue { get; set; }
        public TaskVariableType Type
        {
            get { return TaskVariableType.Static; }
        }

        public string Value { get; set; }

        public string ConnectionString { get; set; }
        public string Statement { get; set; }


        public void Load(DataManager dm)
        {
            OriginalValue = Value;
            Value = Convert.ToString(SQLDataExecutor.ExecuteScalar(ConnectionString, Statement));
        }


        
    }
}
