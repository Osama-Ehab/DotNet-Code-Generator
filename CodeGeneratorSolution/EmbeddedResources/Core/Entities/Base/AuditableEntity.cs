using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Core.Entities.Base
{
    // Now we take TWO types:
    // TKey     = The Entity's ID (e.g. OrderID)
    // TUserKey = The User's ID (e.g. CreatedBy which is an int)
    public abstract class AuditableEntity<TKey, TUserKey> : BaseEntity<TKey>
    {
        public DateTime CreatedDate { get; set; }
        public required TUserKey CreatedBy { get; set; } // Flexible!

        public DateTime? ModifiedDate { get; set; }
        public required TUserKey ModifiedBy { get; set; }
    }
}
