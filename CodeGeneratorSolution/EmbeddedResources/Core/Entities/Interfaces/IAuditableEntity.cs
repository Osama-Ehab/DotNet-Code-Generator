using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Core.Entities.Interfaces
{
    public interface IAuditableEntity<TKey, TUserKey> : IEntity<TKey>
        where TUserKey : struct
{
    public DateTime CreatedDate { get; set; }
    public TUserKey CreatedBy { get; set; } // Flexible!
    public DateTime? ModifiedDate { get; set; }
    public Nullable<TUserKey> ModifiedBy { get; set; }
}


}
