using CodeGeneratorSolution.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
{
    // ====================================================
    // NORMAL READ BY ID (Returns the exact Table DTO)
    // ====================================================
    public interface IReadByIdService<TReadDTO, TKey> : IEntityService<TKey>
    {
        // Notice the name is standard: GetByIdAsync, not GetDetailsAsync
        Task<Result<TReadDTO>> GetByIdAsync(TKey id);
    }
}
