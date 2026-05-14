using CodeGeneratorSolution.Infrastructure;
using CodeGeneratorSolution.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    public interface IDeleteRepository<TKey> : IEntityRepository<TKey>
    {
        Task<bool>  DeleteAsync(TKey id);
    }
}
