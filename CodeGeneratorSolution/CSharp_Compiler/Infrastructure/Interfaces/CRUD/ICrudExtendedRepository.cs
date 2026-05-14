using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Infrastructure.Interfaces
{
    public interface ICrudExtendedRepository<TEntity,TDetailsDTO, TListDTO, TKey> : 
        ICrudRepository<TEntity, TKey> ,
        IReadDetailsRepository<TDetailsDTO, TKey>, IReadListRepository<TListDTO>
    {
       
    }
}
