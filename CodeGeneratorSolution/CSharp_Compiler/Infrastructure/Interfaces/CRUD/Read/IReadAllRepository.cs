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
    // RAW DOMAIN READ (Only for small lookup tables)
    // ====================================================
    public interface IReadAllRepository<TEntity> : IRepository
    {
        Task<IReadOnlyList<TEntity>>  GetAllAsync();
    }
}
