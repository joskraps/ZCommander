using System;
using ZCommander.Shared.Enums;

namespace ZCommander.Core.Models
{
    [Serializable()]
    public class ReplacementValue
    {
        public bool Global { get; set; }
        public ReplaceMethod ReplaceMethod { get; set; }
        public string Pattern { get; set; }
        public string Value { get; set; }
    }
}
