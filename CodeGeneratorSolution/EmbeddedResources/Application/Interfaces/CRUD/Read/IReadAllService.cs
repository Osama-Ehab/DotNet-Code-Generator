using {{TARGET_NAMESPACE}}.Application;
using {{TARGET_NAMESPACE}}.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{
    // ====================================================
    // RAW DOMAIN READ (Only for small lookup tables)
    // ====================================================
    public interface IReadAllService<TReadDTO> : IService
    {
        Task<Result<IEnumerable<TReadDTO>>> GetAllAsync();
    }
}
