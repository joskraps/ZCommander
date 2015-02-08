using System;
using System.Collections.Generic;
using ZCommander.Core.Models;

namespace ZCommander.Core.Data
{
    [Serializable()]
    public class ExternalAssembly
    {
        public Type ObjectType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ObjectName { get; set; }
        public List<ExternalAssemblyObjectProperty> Properties { get; set; }
    }
}
