using CodeGeneratorSolution.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Core
{
    public class MetadataFetcher
    {
        private readonly string _connectionString;

        public MetadataFetcher(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<TableSchema>> GetTablesAsync()
        {
            // 1. Ensure SPs exist (Optional, depending on your setup)
            MetadataInstaller.EnsureStoredProceduresExist(_connectionString);

            var tables = new List<TableSchema>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();            

                // ---------------------------------------------------------
                // STEP 1: Get List of Tables
                // ---------------------------------------------------------
                using (var cmd = new SqlCommand("usp_Meta_GetTables", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tables.Add(new TableSchema
                            {
                                Name = reader["TABLE_NAME"].ToString(),
                                Columns = new List<ColumnDefinition>()
                            });
                        }
                    }
                }

                // ---------------------------------------------------------
                // STEP 2: Loop tables and fetch columns
                // ---------------------------------------------------------
                foreach (var table in tables)
                {
                    table.Columns = await GetColumnsForTableAsync(conn, table.Name);
                    table.GridColumns = await GetStoredProcedureColumnsAsync(conn, $"usp_{table.ClassName}_GetList");
                    table.DetailedColumns = await GetStoredProcedureColumnsAsync(conn, $"usp_{table.ClassName}_GetDetails");

                    LoadColumnsExtendedProperties(conn, table);
                    LoadTableExtendedProperties(conn, table);
                }
            }

            return tables;
        }

        private async Task<List<ColumnDefinition>> GetColumnsForTableAsync(SqlConnection conn, string tableName)
        {
            var columns = new List<ColumnDefinition>();

            using (var cmd = new SqlCommand("usp_Meta_GetTableColumns", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", tableName);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var col = new ColumnDefinition
                        {
                            Name = reader["COLUMN_NAME"].ToString(),
                            SqlType = reader["DATA_TYPE"].ToString(),
                            IsNullable = reader["IS_NULLABLE"].ToString() == "YES",
                            MaxLength = Convert.ToInt32(reader["MaxLen"]),
                            Precision = Convert.ToInt32(reader["Precision"]),
                            Scale = Convert.ToInt32(reader["Scale"]),
                            IsIdentity = Convert.ToBoolean(reader["IsIdentity"]),
                            IsPrimaryKey = Convert.ToBoolean(reader["IsPrimaryKey"]),
                            IsForeignKey = Convert.ToBoolean(reader["IsForeignKey"])
                        };

                        columns.Add(col);
                    }
                }
            }

            return columns;
        }

        private async Task<List<ColumnDefinition>> GetStoredProcedureColumnsAsync(SqlConnection conn, string spName)
        {
            var columns = new List<ColumnDefinition>();

            using (var cmd = new SqlCommand("usp_Meta_GetStoredProcedureColumns", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SpName", spName);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var col = new ColumnDefinition
                        {
                            Name = reader["ColumnName"].ToString(),
                            SqlType = reader["SqlDataType"].ToString(),
                            IsNullable = (bool)reader["IsNullable"]
                        };

                        columns.Add(col);
                    }
                }
            }

            return columns;
        }

      
        private void LoadColumnsExtendedProperties(SqlConnection conn, TableSchema table)
        {
            using (var cmd = new SqlCommand("usp_Meta_GetColumnExtendedProperties", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", table.Name);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader["ColumnName"].ToString();
                        string propName = reader["PropertyName"].ToString();
                        string propValue = reader["PropertyValue"]?.ToString() ?? "";

                        foreach (var colList in new[] { table.Columns, table.GridColumns, table.DetailedColumns })
                        {
                            var col = colList.Find(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                            if (col != null)
                            {
                                col.ExtendedProperties[propName] = propValue;
                            }
                        }
                    }
                }
            }
        }

        private void LoadTableExtendedProperties(SqlConnection conn, TableSchema table)
        {
            using (var cmd = new SqlCommand("usp_Meta_GetTableExtendedProperties", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", table.Name);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string propName = reader["PropertyName"].ToString();
                        string propValue = reader["PropertyValue"]?.ToString() ?? "";

                        table.ExtendedProperties[propName] = propValue;
                    }
                }
            }
        }
    }
}