using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Application.Interfaces
{
   public interface ICrudExtendedService<TCreateDto, TReadDTO , TUpdateDto, TDetailsDTO, TListDTO, TKey> :
        ICrudService<TCreateDto, TReadDTO , TUpdateDto, TKey>,
        IReadDetailsService<TDetailsDTO, TKey>, IReadListService<TListDTO>
{

}

}
