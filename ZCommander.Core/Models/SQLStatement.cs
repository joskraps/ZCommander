using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Output;
using ZCommander.Shared.Enums;

namespace ZCommander.Models
{
    public class SQLStatement
    {
        public string ConnectionString { get; set; }
        public string Statement { get; set; }
        public List<ITaskOutput> OutputList { get; set; }
        public SqlStatementOutputType OutputType { get; set; }
    }
}
