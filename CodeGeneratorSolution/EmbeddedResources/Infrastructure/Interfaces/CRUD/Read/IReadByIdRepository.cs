using {{TARGET_NAMESPACE}}.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Infrastructure.Interfaces
{
    // ====================================================
    // NORMAL READ BY ID (Returns the exact Table entity)
    // ====================================================
    public interface IReadByIdRepository<TEntity, TKey> : IEntityRepository<TKey>
    {
        // Notice the name is standard: GetByIdAsync, not GetDetailsAsync
        Task<TEntity> GetByIdAsync(TKey id);
    }
}
