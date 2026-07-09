using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Repositories
{
    public abstract class BaseRepository 
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // دالة مركزية لإنشاء الاتصال، ليتم استخدامها مع Dapper في المستودعات الموروثة
        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}