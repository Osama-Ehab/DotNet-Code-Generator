using CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Application.Interfaces
{
    // Bundles all operations for standard tables
    public interface ICrudService<TCreateDto,TReadDTO, TUpdateDto, TKey> :
        ICreateService<TCreateDto, TKey>,
        IReadByIdService<TReadDTO, TKey>,
        IReadAllService<TReadDTO>,
        IUpdateService<TUpdateDto, TKey>,
        IDeleteService<TKey>,
        IExistsService<TKey>
    {
    }
}
