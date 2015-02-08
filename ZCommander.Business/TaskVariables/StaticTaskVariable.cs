using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Core.Managers;
using ZCommander.Models;
using ZCommander.Shared.Enums;
using ZCommander.Core.Tasks;

namespace ZCommander.Business.TaskVariables
{
    public class StaticTaskVariable:ITaskVariable
    {
        public string Name { get; set; }
        public bool Reset { get; set; }
        public TaskVariableType Type
        {
            get { return TaskVariableType.Static; }
        }

        public string OriginalValue { get; set; }
        public string Value { get; set; }


        public void Load(DataManager dm)
        {
            OriginalValue = Value;
            Value = dm.GetValueFromMacro(Value);
        }
    }
}
