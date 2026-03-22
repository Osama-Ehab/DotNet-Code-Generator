// Infrastructure/BaseRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CodeGeneratorSolution.EmbeddedResources.Infrastructure.Interfaces;



namespace CodeGeneratorSolution.EmbeddedResources.Templates.Infrastructure
{
    // Note: We do NOT implement IBaseRepository here. 
    // We just provide the protected tools.
    public abstract class BaseRepository<T, TKey>  
    {
        private readonly string _connectionString;

        public BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- 1. TOOL: GENERIC ADD ---
        protected async Task<TKey> AddAsync(string spName, SqlParameter[] parameters, string outParamName = "@NewId")
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);

                // Configure Output
                var outParam = new SqlParameter(outParamName, SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (TKey)outParam.Value;
            }
        }

        // --- 2. TOOL: GENERIC UPDATE/DELETE ---
        protected async Task<bool> ExecuteNonQueryAsync(string spName, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        // --- 3. TOOL: GENERIC GET LIST ---
        protected async Task<List<T>> GetListAsync(string spName, Func<IDataReader, T> mapper, SqlParameter[] parameters = null)
        {
            var list = new List<T>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
   
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(mapper(reader));
                    }
                }
            }
            return list;
        }

        // --- 4. TOOL: GENERIC GET SINGLE ---
        protected async Task<T> GetSingleAsync(string spName, SqlParameter parameter, Func<IDataReader, T> mapper)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(parameter);
                // Configure Output

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return mapper(reader);
                    }
                }
            }
            return default(T);
        }

        // --- 5. TOOL: CHECK EXISTENCE (Scalar is cleaner) ---
        protected async Task<bool> ExistsAsync(string spName, SqlParameter parameter)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(parameter);

                await conn.OpenAsync();
                // Assumes SP does "SELECT 1 WHERE ..."
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }
    }
}