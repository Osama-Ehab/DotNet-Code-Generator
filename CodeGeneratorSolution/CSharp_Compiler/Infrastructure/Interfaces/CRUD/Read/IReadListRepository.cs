using CodeGeneratorSolution.Infrastructure;
using CodeGeneratorSolution.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    // ====================================================
    // LIST READ: Pure ISP Compliance. NO TKEY!
    // ====================================================
    // It inherits the base IRepository, so DI still finds it, but it drops TKey.
    public interface IReadListRepository<TListDTO> : IRepository
    {
        Task<IReadOnlyList<TListDTO>> GetListAsync();
    }

}
