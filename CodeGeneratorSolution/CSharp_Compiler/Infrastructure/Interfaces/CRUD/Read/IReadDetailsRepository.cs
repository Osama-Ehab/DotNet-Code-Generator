using CodeGeneratorSolution.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{

    // ====================================================
    // DETAILS READ: Requires TKey
    // ====================================================
    // It inherits IEntityRepository because fetching details requires the ID.
    public interface IReadDetailsRepository<TDetailsDTO, TKey> : IEntityRepository<TKey>
    {
        Task<TDetailsDTO>  GetDetailsAsync(TKey id);
    }

}
