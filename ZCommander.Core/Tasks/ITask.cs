using System.Collections.Generic;
using ZCommander.Core.Logging;
using ZCommander.Core.Managers;
using ZCommander.Models;
using ZCommander.Output;
using ZCommander.Shared.Enums;
using ZCommander.Core.Tasks;

namespace ZCommander.Core.Tasks
{
    public interface ITask
    {
        string Name { get; set; }
        int Sequence { get; set; }
        TaskType Type { get;}

        void Prepare(ref Log returnLog);
        List<Log> Execute(DataManager dm, Dictionary<string, ITaskVariable> taskVariables, Log returnLog);
    }
}
