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
    // RAW DOMAIN READ (Only for small lookup tables)
    // ====================================================
    public interface IReadAllService<TReadDTO> : IService
    {
        Task<Result<IEnumerable<TReadDTO>>> GetAllAsync();
    }
}
