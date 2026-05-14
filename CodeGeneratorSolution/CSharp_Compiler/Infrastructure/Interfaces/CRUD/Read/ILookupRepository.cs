using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    // ====================================================
    // LOOKUP Repository (For static reference tables)
    // ====================================================
    public interface ILookupRepository<TEntity, TKey> :
        IReadAllRepository<TEntity>,          // Provides: GetAllAsync()
        IReadByIdRepository<TEntity, TKey>    // Provides: GetByIdAsync(id)
    {
    }
}
