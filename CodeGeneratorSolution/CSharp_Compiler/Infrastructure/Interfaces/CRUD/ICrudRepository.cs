using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    // Bundles all operations for standard tables
    public interface ICrudRepository< TEntity, TKey> :
        ICreateRepository<TEntity, TKey>,
        IReadByIdRepository<TEntity, TKey>,
        IReadAllRepository<TEntity>,
        IUpdateRepository<TEntity, TKey>,
        IDeleteRepository<TKey>,
        IExistsRepository<TKey>
    {
    }
}
