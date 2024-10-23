using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace PCNW.ExtentionMethods
{
    public static class GeneralMethods
    {
        public static async Task<List<dynamic>> DynamicListFromSqlAsync(this DbContext db, string Sql, Dictionary<string, object> Params)
        {
            List<dynamic> response = new();
            DbCommand? cmd = db.Database.GetDbConnection().CreateCommand();
            if (cmd != null)
            {
                cmd.CommandText = $"{Sql}";
                // if sql is stored procedure then set and then add paramertes
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Connection != null)
                {
                    if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }
                    foreach (KeyValuePair<string, object> p in Params)
                    {
                        DbParameter dbParameter = cmd.CreateParameter();
                        dbParameter.DbType = DbType.String;
                        dbParameter.ParameterName = p.Key;
                        dbParameter.Value = p.Value;
                        cmd.Parameters.Add(dbParameter);
                    }

                    DbDataReader? dataReader = await cmd.ExecuteReaderAsync();
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            {
                                row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                            }
                            response.Add(row);
                        }
                    }
                    //dataReader.Close();
                }
            }
            return response;
        }

        public static IEnumerable<dynamic> DynamicListFromSql(this DbContext db, string Sql, Dictionary<string, object> Params)
        {
            DbCommand? cmd = db.Database.GetDbConnection().CreateCommand();
            if (cmd != null)
            {
                cmd.CommandText = $"{Sql}";
                // if sql is stored procedure then set and then add paramertes
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Connection != null)
                {
                    if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }
                    foreach (KeyValuePair<string, object> p in Params)
                    {
                        DbParameter dbParameter = cmd.CreateParameter();
                        dbParameter.DbType = DbType.String;
                        dbParameter.ParameterName = p.Key;
                        dbParameter.Value = p.Value;
                        cmd.Parameters.Add(dbParameter);
                    }

                    DbDataReader? dataReader = cmd.ExecuteReader();
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            {
                                row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                            }
                            yield return row;
                        }
                    }
                    dataReader.Close();
                }
            }
        }

        public static string GetErrorCode(Exception ex)
        {
            int code = 0;
            Win32Exception w32ex = ex as Win32Exception;
            if (w32ex == null)
            {
                w32ex = ex.InnerException as Win32Exception;
            }
            if (w32ex != null)
            {
                code = w32ex.ErrorCode;
            }
            return code.ToString();
        }

        public static string fn_Date_Formatter(string Param_DateTime)
        {
            string result = Param_DateTime;
            if (!string.IsNullOrEmpty(Param_DateTime))
            {
                DateTime dateTime = new();
                bool isDate = DateTime.TryParse(Param_DateTime, out dateTime);
                if (isDate)
                    result = dateTime.ToString("MM/dd/yyyy");
            }
            return result;
        }
    }
}
