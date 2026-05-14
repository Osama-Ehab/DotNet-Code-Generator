using CodeGeneratorSolution.Application;
using CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    public interface ICreateService<TCreateDto, TKey> : IEntityService<TKey>
    {
        Task<Result<TKey>> AddAsync(TCreateDto entity);
    }
}
