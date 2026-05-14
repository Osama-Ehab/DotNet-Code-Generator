using {{TARGET_NAMESPACE}}.Infrastructure;
using {{TARGET_NAMESPACE}}.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Infrastructure.Interfaces
{
    public interface IUpdateRepository<TEntity, TKey> : IEntityRepository<TKey>
    {
          Task<bool> UpdateAsync(TEntity entity);
    }
}
