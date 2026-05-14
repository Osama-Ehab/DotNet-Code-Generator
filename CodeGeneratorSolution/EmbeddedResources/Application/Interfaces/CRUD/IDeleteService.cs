using {{TARGET_NAMESPACE}}.Application;
using {{TARGET_NAMESPACE}}.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{
    public interface IDeleteService<TKey> : IEntityService<TKey>
    {
        Task<Result<bool>> DeleteAsync(TKey id);
    }
}
