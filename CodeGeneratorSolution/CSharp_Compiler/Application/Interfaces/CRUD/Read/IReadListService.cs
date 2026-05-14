using CodeGeneratorSolution.Application;
using CodeGeneratorSolution.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
{
    // ====================================================
    // LIST READ: Pure ISP Compliance. NO TKEY!
    // ====================================================
    // It inherits the base IService, so DI still finds it, but it drops TKey.
    public interface IReadListService<TListDTO> : IService
    {
        Task<Result<IReadOnlyList<TListDTO>>> GetListAsync();
    }

}
