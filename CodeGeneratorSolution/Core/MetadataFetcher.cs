using CodeGeneratorSolution.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
            // MetadataInstaller.EnsureStoredProceduresExist(_connectionString);

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
                // STEP 2: Loop tables and fetch columns & metadata
                // ---------------------------------------------------------
                foreach (var table in tables)
                {
                    table.Columns = await GetColumnsForTableAsync(conn, table.Name);

                    // 💡 الأهم: قراءة الـ Metadata من الجدول الذكي
                    await LoadUIColumnMetadataAsync(conn, table);

                    // تحويل الدالة لتعمل بشكل غير متزامن (Async) لتوحيد الأداء
                    await LoadTableExtendedPropertiesAsync(conn, table);
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
                        string columnName = reader["COLUMN_NAME"].ToString();

                        var col = new ColumnDefinition
                        {
                            // تسوية اسم حقل الـ ID
                            Name = (columnName == "ID") ? "Id" : columnName,
                            SqlType = reader["DATA_TYPE"].ToString(),
                            IsNullable = reader["IS_NULLABLE"].ToString() == "YES",

                            // الحماية ضد الـ DBNull
                            MaxLength = reader["MaxLen"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MaxLen"]),
                            Precision = reader["Precision"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Precision"]),
                            Scale = reader["Scale"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Scale"]),
                            IsIdentity = reader["IsIdentity"] != DBNull.Value && Convert.ToBoolean(reader["IsIdentity"]),
                            IsPrimaryKey = reader["IsPrimaryKey"] != DBNull.Value && Convert.ToBoolean(reader["IsPrimaryKey"]),
                            IsForeignKey = reader["IsForeignKey"] != DBNull.Value && Convert.ToBoolean(reader["IsForeignKey"])
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

            // فحص سريع إذا كان الإجراء المخزن غير موجود لمنع الأخطاء
            using (var checkCmd = new SqlCommand("SELECT OBJECT_ID(@SpName)", conn))
            {
                checkCmd.Parameters.AddWithValue("@SpName", spName);
                if (await checkCmd.ExecuteScalarAsync() == DBNull.Value)
                    return columns;
            }

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

        // =========================================================================================
        // 🚀 تحديث المعمارية: قراءة جدول UI_ColumnMetadata بطريقة نظيفة جداً
        // =========================================================================================
        private async Task LoadUIColumnMetadataAsync(SqlConnection conn, TableSchema table)
        {
            string query = "SELECT * FROM [dbo].[UI_ColumnMetadata] WHERE TableName = @TableName";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", table.Name);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string dbColumnName = reader["ColumnName"].ToString();
                        string normalizedColumnName = (dbColumnName == "ID") ? "Id" : dbColumnName;

                        // لا حاجة لعمل Foreach على القوائم الأخرى! بمجرد تحديث Columns ستتحدث البقية آلياً.
                        var col = table.Columns.Find(c => c.Name.Equals(normalizedColumnName, StringComparison.OrdinalIgnoreCase));

                        if (col != null)
                        {
                            // 1. تعيين النصوص
                            col.MetaDisplayNameAr = reader["MetaDisplayNameAr"] as string;
                            col.MetaDisplayNameEn = reader["MetaDisplayNameEn"] as string;
                            col.MetaControlType = reader["MetaControlType"] as string;
                            col.MetaIconCode = reader["MetaIconCode"] as string;
                            col.MetaAllowedValues = reader["MetaAllowedValues"] as string;

                            // 2. تعيين المتغيرات المنطقية (Boolean) باستخدام الدوال المساعدة
                            col.MetaIsSensitive = GetNullableBool(reader["MetaIsSensitive"]);
                            col.MetaIsAuditable = GetNullableBool(reader["MetaIsAuditable"]);
                            col.MetaIncludeInInsert = GetNullableBool(reader["MetaIncludeInInsert"]);
                            col.MetaIncludeInUpdate = GetNullableBool(reader["MetaIncludeInUpdate"]);
                            col.MetaIsPassword = GetNullableBool(reader["MetaIsPassword"]);
                            col.MetaIsSearchable = GetNullableBool(reader["MetaIsSearchable"]);
                            col.MetaIsUniqueSelector = GetNullableBool(reader["MetaIsUniqueSelector"]);
                            col.MetaIsLookupColumn = GetNullableBool(reader["MetaIsLookupColumn"]);

                            // 3. تعيين إعدادات التنسيق والهيكلة المرئية (Layout)
                            col.UIRow = GetNullableInt(reader["UIRow"]);
                            col.UIColumn = GetNullableInt(reader["UIColumn"]);
                            col.UIColSpan = GetNullableInt(reader["UIColSpan"]);
                        }
                    }
                }
            }
        }

        private async Task LoadTableExtendedPropertiesAsync(SqlConnection conn, TableSchema table)
        {
            using (var cmd = new SqlCommand("usp_Meta_GetTableExtendedProperties", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", table.Name);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string propName = reader["PropertyName"].ToString();
                        string propValue = reader["PropertyValue"]?.ToString() ?? "";

                        table.ExtendedProperties[propName] = propValue;
                    }
                }
            }
        }

        // =========================================================================================
        // 🛠️ دوال مساعدة (Helper Methods) لتنظيف الكود من التكرار المزعج
        // =========================================================================================
        private bool? GetNullableBool(object dbValue)
        {
            return dbValue == DBNull.Value ? (bool?)null : Convert.ToBoolean(dbValue);
        }

        private int? GetNullableInt(object dbValue)
        {
            return dbValue == DBNull.Value ? (int?)null : Convert.ToInt32(dbValue);
        }
    }
}