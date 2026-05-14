using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
{
    // ====================================================
    // LOOKUP SERVICE (For static reference tables)
    // ====================================================
    public interface ILookupService<TReadDTO, TKey> :
        IReadAllService<TReadDTO>,          // Provides: GetAllAsync()
        IReadByIdService<TReadDTO, TKey>    // Provides: GetByIdAsync(id)
    {
    }
}
