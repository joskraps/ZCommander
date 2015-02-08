using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Factories
{
    class NumberIncrementorFactory : IDataFactory
    {
        public DataFactoryType Type
        {
            get {return DataFactoryType.NUMBERINCREMENTOR; }
        }

        public bool RefreshValues { get; set; }
        public int RefreshRate { get; set; }
        public string Name { get; set; }
        public int Seed { get; set; }


        private object SyncLockObject = new object();

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
            lock (SyncLockObject)
            {
                var returnValue = Convert.ToInt32(Data.Tables[Convert.ToInt32(TableIndex)].Rows[0][Convert.ToInt32(ColumnName)]);
                Data.Tables[Convert.ToInt32(TableIndex)].Rows[0][Convert.ToInt32(ColumnName)] = returnValue + Seed;
                return returnValue;
            }
        }
    }
}
