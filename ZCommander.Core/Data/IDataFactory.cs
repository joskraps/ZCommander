using System.Data;
using ZCommander.Core.Logging;
using ZCommander.Shared.Enums;

namespace ZCommander.Core.Data
{
    public interface IDataFactory
    {
        DataFactoryType Type { get; }
        bool RefreshValues { get; set; }
        int RefreshRate { get; set; }
        string Name { get; set; }
        DataSet Data { get; set; }

        void Load(ref Log returnLog);
        void Refresh(ref Log returnLog);
        object GetValue(string TableIndex, string ColumnName, string Method);
    }
}
