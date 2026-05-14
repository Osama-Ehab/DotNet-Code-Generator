using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Infrastructure.Interfaces
{
    // ====================================================
    // EXISTS (For quick validation without loading data)
    // ====================================================
    public interface IExistsRepository<TKey> : IEntityRepository<TKey>
    {
        // Note: I am using Task<bool> exactly as you requested. 
        // If you prefer to keep the error-handling pattern, you could 
        // also use bool, but Task<bool> is often preferred 
        // for simple true/false existence checks.
        Task<bool> ExistsAsync(TKey id);
    }
}
