using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Shared.Enums;

namespace ZCommander.Output
{
    public interface ITaskOutput
    {
        TaskOutputType Type { get; }
        string Target { get; set; }

        void Execute();
    }
}
