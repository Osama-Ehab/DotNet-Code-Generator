using CodeGeneratorSolution.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace School.Infrastructure
{
    // 1. We remove all the DTO generics from the class level!
    // We can keep TEntity and TKey if you want to use them for standard CRUD,
    // but for the ADO.NET tools, we don't even need them.
    public abstract class BaseRepository : IRepository
    {
        private readonly string _connectionString;

        public BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- 1. TOOL: GENERIC ADD ---
        // Notice we added <TKey> to the method!
        protected async Task<TKey> AddAsync<TKey>(string spName, SqlParameter[] parameters, string outParamName = "@NewId")
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);

                var outParam = new SqlParameter(outParamName, SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (TKey)outParam.Value;
            }
        }

        // --- 2. TOOL: GENERIC GET LIST ---
        // Notice we added <T> to the method! It can return ANY DTO.
        protected async Task<IReadOnlyList<T>> GetListAsync<T>(string spName, Func<IDataReader, T> mapper, SqlParameter[] parameters = null)
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

        // --- 3. TOOL: GENERIC GET SINGLE ---
        // Notice we added <T> to the method! 
        protected async Task<T> GetSingleAsync<T>(string spName, SqlParameter parameter, Func<IDataReader, T> mapper)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameter != null) cmd.Parameters.Add(parameter);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return mapper(reader); // Maps the exact DTO you requested
                    }
                }
            }
            // Return null if not found (using default constraint)
            return default(T);
        }
        // --- 4. TOOL: GENERIC UPDATE/DELETE ---
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