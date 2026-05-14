using CodeGeneratorSolution.Application;
using CodeGeneratorSolution.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
{
    public interface IDeleteService<TKey> : IEntityService<TKey>
    {
        Task<Result<bool>> DeleteAsync(TKey id);
    }
}
