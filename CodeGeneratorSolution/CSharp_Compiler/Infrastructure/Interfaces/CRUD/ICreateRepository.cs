using CodeGeneratorSolution.Infrastructure;
using CodeGeneratorSolution.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    public interface ICreateRepository<TEntity, TKey> : IEntityRepository<TKey>
    {
        Task<TKey> AddAsync(TEntity entity);
    }
}
