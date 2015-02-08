using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ZCommander.Core.Data
{
    public static class SQLDataExecutor
    {
        public static DataSet FillDataSet(string connection, string commandtext)
        {
            DataSet returnSet = new DataSet();
            SqlConnection _connection = new SqlConnection(connection);
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = _connection.CreateCommand();
            cmd = _connection.CreateCommand();
            cmd.CommandText = commandtext;
            da.SelectCommand = cmd;

            _connection.Open();
            try
            {
                da.Fill(returnSet);
                return returnSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            } 
        }

        public static object ExecuteStatement(string connection, string statement)
        {
            return ExecuteStatement(connection, statement, 30);
        }

        public static object ExecuteStatement(string connection, string statement,int timeout)
        {
            SqlConnection _connection = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(statement, _connection);
            cmd.CommandTimeout = timeout;
            _connection.Open();
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
        }

        public static DataSet ExecuteStatement(string connection, string procedure, List<SqlParameter> parameters)
        {

            DataSet returnSet = new DataSet();
            SqlDataAdapter sqlDA = new SqlDataAdapter(procedure, new SqlConnection(connection));

            sqlDA.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (SqlParameter par in parameters)
                {
                    sqlDA.SelectCommand.Parameters.Add(par);
                }
            }

            sqlDA.Fill(returnSet);

            return returnSet;
        }

        public static object ExecuteScalar(string connection, string statement)
        {
            return GetValue(connection, statement, 30);
        }
        public static object GetValue(string connection, string statement, int timeout)
        {
            SqlConnection _connection = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand(statement, _connection);
            cmd.CommandTimeout = timeout;
            _connection.Open();
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }
        }
    }
}
