using CodeGeneratorSolution.Templetes.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Application.Interfaces
{
    public interface IService<T, TKey>
    {
        Task<Result<IEnumerable<T>>> GetAllAsync();
        Task<Result<T>> GetByIdAsync(TKey id);
        Task<Result<TKey>> AddAsync(T dto);
        Task<Result<bool>> UpdateAsync(T dto);
        Task<Result<bool>> DeleteAsync(TKey id);

        Task<bool> ExistsAsync(TKey id);

    }
}
