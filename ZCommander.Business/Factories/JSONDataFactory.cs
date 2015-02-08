using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using ZCommander.Core.Data;
using ZCommander.Core.Logging;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Factories
{
    public class JSONDataFactory : IDataFactory
    {
        private Random _generator;

        public DataFactoryType Type
        {
            get { return DataFactoryType.JSON; }
        }

        public bool RefreshValues { get; set; }

        public int RefreshRate { get; set; }

        public string Name { get; set; }

        public string LoadPath { get; set; }

        public DataSet Data { get; set; }

        public JSONDataFactory()
        {
            _generator = new Random();
        }

        public void Load(ref Log returnLog)
        {

            if (LoadPath.Contains("http://")) { throw new NotImplementedException(); }
            else
            {
                using (StreamReader r = new StreamReader(LoadPath))
                {
                    string json = r.ReadToEnd();
                    DataTable jsonDT = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                    Data = new DataSet();
                    Data.Tables.Add(jsonDT);
                }
            }
        }

        public void Refresh(ref Log returnLog)
        {


        }

        public object GetValue(string TableIndex, string ColumnName, string Method)
        {
            var returnValue = "";
            switch (Method)
            {
                case "RAND":
                    {
                        var rowCount = this.Data.Tables[Convert.ToInt32(TableIndex)].Rows.Count;
                        int randomNumber = _generator.Next(0, rowCount);
                        returnValue = Convert.ToString(Data.Tables[Convert.ToInt32(TableIndex)].Rows[randomNumber][ColumnName]);
                        return returnValue;
                    }
                case "MAX":
                    return this.Data.Tables[Convert.ToInt32(TableIndex)].AsEnumerable()
                        .Max(row => row[ColumnName]);
                case "MIN":
                    return this.Data.Tables[Convert.ToInt32(TableIndex)].AsEnumerable()
                        .Min(row => row[ColumnName]);
            }
            return null;
        }
    }
}
