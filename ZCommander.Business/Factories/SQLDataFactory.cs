using System;
using System.Data;
using System.Linq;
using ZCommander.Core.Logging;
using ZCommander.Core.Data;
using ZCommander.Shared.Enums;

namespace ZCommander.Business.Factories
{
    [Serializable()]
    public class SQLDataFactory : IDataFactory
    {

        private Random _generator;
        public DataFactoryType Type
        {
            get
            {
                return DataFactoryType.SQL;
            }
        }
        public bool RefreshValues { get; set; }
        public int RefreshRate { get; set; }
        public string Name { get; set; }

        public string ConnectionString { get; set; }
        public string FillQuery { get; set; }

        public DataSet Data { get; set; }

        public SQLDataFactory()
        {
            _generator = new Random();
        }

        public void Load(ref Log returnLog)
        {
            Data = new DataSet();



            try
            {
                returnLog.StartTime = DateTime.Now;

                Data = SQLDataExecutor.FillDataSet(this.ConnectionString, this.FillQuery);
                returnLog.Message = "Load Completed successfully. Record count:" + Data.Tables[0].Rows.Count;
                returnLog.EndTime = DateTime.Now;


            }
            catch (Exception ex)
            {
                returnLog.EndTime = DateTime.Now;
                returnLog.Successful = false;
                returnLog.Message = ex.Message;
            }

        }


        public void Refresh(ref Log returnLog)
        {
            DataSet updatedDataSet = new DataSet();
            try
            {
                returnLog.StartTime = DateTime.Now;

                updatedDataSet = SQLDataExecutor.FillDataSet(this.ConnectionString, this.FillQuery);
                returnLog.Message = "Refresh Completed successfully. Previous count:" + Data.Tables[0].Rows.Count + ". New count:" + updatedDataSet.Tables[0].Rows.Count;

                Data = updatedDataSet.Copy();

                returnLog.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                returnLog.EndTime = DateTime.Now;
                returnLog.Successful = false;
                returnLog.Message = "Error in refresh:" + ex.Message;
            }
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
                case "PULL":
                        var rowCount2 = this.Data.Tables[Convert.ToInt32(TableIndex)].Rows.Count;
                        int randomNumber2 = _generator.Next(0, rowCount2);
                        returnValue = Convert.ToString(Data.Tables[Convert.ToInt32(TableIndex)].Rows[randomNumber2][ColumnName]);
                        return returnValue;
            }
            return null;
        }
    }
}
