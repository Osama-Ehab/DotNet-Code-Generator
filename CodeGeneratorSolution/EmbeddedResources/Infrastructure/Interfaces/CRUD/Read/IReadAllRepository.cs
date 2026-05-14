using {{TARGET_NAMESPACE}}.Infrastructure;
using {{TARGET_NAMESPACE}}.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Infrastructure.Interfaces
{
    // ====================================================
    // RAW DOMAIN READ (Only for small lookup tables)
    // ====================================================
    public interface IReadAllRepository<TEntity> : IRepository
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();
    }
}
