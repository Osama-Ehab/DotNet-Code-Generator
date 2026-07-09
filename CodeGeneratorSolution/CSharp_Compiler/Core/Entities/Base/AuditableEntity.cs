using CodeGeneratorSolution.CSharp_Compiler.Core.Entities;
using CodeGeneratorSolution.CSharp_Compiler.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Core.Entities.Base
{
    // Now we take TWO types:
    // TKey     = The Entity's ID (e.g. OrderID)
    // TUserKey = The User's ID (e.g. CreatedBy which is an int)
    public abstract class AuditableEntity<TKey, TUserKey> : Entity<TKey>, IAuditableEntity<TKey, TUserKey>
    {
        // بيانات الإنشاء (إلزامية)
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public TUserKey CreatedBy { get; set; }

        // بيانات التعديل (اختيارية - Nullable)
        public DateTime? ModifiedDate { get; set; }
        public TUserKey? ModifiedBy { get; set; }

        // الحذف الناعم (مفعل افتراضياً)
        public bool IsActive { get; set; } = true;
    }
}
