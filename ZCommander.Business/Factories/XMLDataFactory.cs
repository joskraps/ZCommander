using System;
using System.Data;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Factories
{
    public class XMLDataFactory : IDataFactory
    {
        public DataFactoryType Type
        {
            get { return DataFactoryType.XML; }
        }

        public bool RefreshValues { get; set; }

        public int RefreshRate { get; set; }

        public string Name { get; set; }

        public DataSet Data { get; set; }

        public void Load(ref Log returnLog)
        {
            throw new NotImplementedException();
        }

        public void Refresh(ref Log returnLog)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string TableIndex, string ColumnName, string Method)
        {
            throw new NotImplementedException();
        }
    }
}
