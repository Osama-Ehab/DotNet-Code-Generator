using {{TARGET_NAMESPACE}}.Core.Entities;
using {{TARGET_NAMESPACE}}.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.Core.Entities.Base
{
    // Now we take TWO types:
    // TKey     = The Entity's ID (e.g. OrderID)
    // TUserKey = The User's ID (e.g. CreatedBy which is an int)
    public abstract class AuditableEntity<TKey, TUserKey> : Entity<TKey>, IAuditableEntity<TKey, TUserKey>
        where TUserKey : struct
{
    public DateTime CreatedDate { get; set; }
    public TUserKey CreatedBy { get; set; } // Flexible!

    public Nullable<DateTime> ModifiedDate { get; set; }
    public Nullable<TUserKey> ModifiedBy { get; set; }
}
}
