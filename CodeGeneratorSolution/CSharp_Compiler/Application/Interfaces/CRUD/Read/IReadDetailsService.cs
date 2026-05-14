using CodeGeneratorSolution.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
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
