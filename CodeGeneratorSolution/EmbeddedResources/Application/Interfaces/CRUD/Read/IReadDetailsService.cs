using {{TARGET_NAMESPACE}}.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{

    // ====================================================
    // DETAILS READ: Requires TKey
    // ====================================================
    // It inherits IEntityService because fetching details requires the ID.
    public interface IReadDetailsService<TDetailsDTO, TKey> : IEntityService<TKey>
    {
        Task<Result<TDetailsDTO>> GetDetailsAsync(TKey id);
    }

}
