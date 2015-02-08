using System;

namespace ZCommander.Core.Models
{
    [Serializable()]
    public class ExternalAssemblyObjectProperty
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
