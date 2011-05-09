using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace TypeEmitAdministrator
{
    /// <summary>
    /// Please wash your eyes after seeing this...
    /// Todo: REWRITE!
    /// </summary>
    public static class DbExtensionManager
    {
        private readonly static IDictionary<Type, string> TypeMapping =
            new Dictionary<Type, string>()
                {
                    {typeof(string), "nvarchar(50)"},
                    {typeof(DateTime), "DateTime NOT NULL"},
                    {typeof(DateTime?), "DateTime NULL"},
                    {typeof(decimal), "decimal NOT NULL"},
                    {typeof(decimal?), "decimal NULL"},
                    {typeof(Guid), "binary NOT NULL"},
                    {typeof(Guid?), "binary NULL"}
                };


        public static void CreateExtensionTables(IEnumerable<ExtendedTable> extendedTables)
        {
            using (var scope = new TransactionScope())
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlExpress"].ConnectionString))
            {
                connection.Open();

                foreach (var extendedTable in extendedTables)
                {
                    CreateExtensionTable(connection, extendedTable);
                }

                scope.Complete();
            }
        }

        private static void CreateExtensionTable(DbConnection connection, ExtendedTable extendedTable)
        {
            using (var queryCommand = connection.CreateCommand())
            {
                string primaryKeyColumn = GetPrimaryKey(connection, extendedTable.BaseTableName);

                queryCommand.CommandText = string.Format(
                    "SELECT {0} FROM {1}",
                    primaryKeyColumn,
                    extendedTable.BaseTableName);

                string dataType = null;
                string properties =
                    string.Join(
                        ", ",
                        new [] { string.Format("{0} uniqueidentifier PRIMARY KEY", primaryKeyColumn) }
                        .Union(
                            from property in extendedTable.ExtendedProperties
                            where TypeMapping.TryGetValue(property.PropertyType, out dataType)
                            select string.Format("{0} {1}", property.PropertyName, dataType)));

                var creationCommand = connection.CreateCommand();
                creationCommand.CommandText = string.Format(
                    "CREATE TABLE {0} ({1});",
                    extendedTable.TableName,
                    properties);
                creationCommand.ExecuteNonQuery();

                using (DbDataReader reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object keyValue = reader[primaryKeyColumn];

                        creationCommand = connection.CreateCommand();
                        creationCommand.CommandText = string.Format(
                            "INSERT INTO {0} ({1}) VALUES (@value);",
                            extendedTable.TableName,
                            primaryKeyColumn);

                        var parameter = creationCommand.CreateParameter();
                        parameter.DbType = DbType.Guid;
                        parameter.ParameterName = "@value";
                        parameter.Value = keyValue;
                        
                        creationCommand.Parameters.Add(parameter);

                        creationCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string GetPrimaryKey(DbConnection connection, string tableName)
        {
            var command = connection.CreateCommand();
            command.CommandText = "sp_pkeys";
            command.CommandType = CommandType.StoredProcedure;

            var parameter = command.CreateParameter();
            parameter.DbType = DbType.String;
            parameter.ParameterName = "@table_name";
            parameter.Value = tableName;
            
            command.Parameters.Add(parameter);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader[3].ToString();
                }

                return null;
            }
        }
    }
}
