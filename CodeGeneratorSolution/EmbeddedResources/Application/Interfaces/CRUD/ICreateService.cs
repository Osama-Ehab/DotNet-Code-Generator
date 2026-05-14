using {{TARGET_NAMESPACE}}.Application;
using {{TARGET_NAMESPACE}}.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Infrastructure.Interfaces
{
    public interface ICreateService<TCreateDto, TKey> : IEntityService<TKey>
    {
          Task<Result<TKey>> AddAsync(TCreateDto entity);
    }
}

