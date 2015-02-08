using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Output;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Output
{
    public class TaskVariableOutput : ITaskOutput
    {
        public TaskOutputType Type
        {
            get { return TaskOutputType.TaskVariable; }
        }

        public string Target { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
