using {{TARGET_NAMESPACE}}.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{
    // Bundles all operations for standard tables
   public interface ICrudService<TCreateDto, TReadDTO, TUpdateDto, TKey> :
        ICreateService<TCreateDto, TKey>,
        IReadByIdService<TReadDTO, TKey>,
        IReadAllService<TReadDTO>,
        IUpdateService<TUpdateDto, TKey>,
        IDeleteService<TKey>,
        IExistsService<TKey>
{
}
}
