using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Core.Managers;
using ZCommander.Shared.Enums;

namespace ZCommander.Core.Tasks
{
    public interface ITaskVariable
    {
        string Name { get; set; }
        TaskVariableType Type { get; }
        string OriginalValue { get; set; }
        string Value { get; set; }
        bool Reset { get; set; }

        void Load(DataManager dm);
    }
}
