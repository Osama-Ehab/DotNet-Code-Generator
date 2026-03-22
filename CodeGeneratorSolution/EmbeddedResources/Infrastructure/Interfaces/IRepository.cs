using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Infrastructure.Interfaces
{
    // The "T" is the Entity (User, Product)
    // The "TKey" is the ID type (int, long, Guid)
    public interface IRepository<T, TKey>
    {
        // 1. Read
        Task<T> GetByIdAsync(TKey id);
        Task<List<T>> GetAllAsync();

        // 2. Write
        Task<TKey> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(TKey id);
        SqlParameter[] GetParameters(T entity, bool isUpdate);
        Task<bool> ExistsAsync(TKey id);
    }
}
