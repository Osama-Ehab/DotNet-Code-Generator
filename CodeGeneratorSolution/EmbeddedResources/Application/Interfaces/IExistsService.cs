using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{TARGET_NAMESPACE}}.Application;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{
    // ====================================================
    // EXISTS (For quick validation without loading data)
    // ====================================================
    public interface IExistsService<TKey> : IEntityService<TKey>
    {
    // Note: I am using Task<bool> exactly as you requested. 
    // If you prefer to keep the error-handling pattern, you could 
    // also use Task<Result<bool>>, but Task<bool> is often preferred 
    // for simple true/false existence checks.
    Task<Result<bool>> ExistsAsync(TKey id);
    }
}
