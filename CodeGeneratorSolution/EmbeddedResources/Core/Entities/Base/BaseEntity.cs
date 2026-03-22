using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Core.Entities.Base
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }

        public override bool Equals(object obj)
        {
            // 1. Check for null
            if (obj == null)
                return false;

            // 2. Optimization: Check for Reference Equality first
            // If they point to the same memory address, they are definitely equal.
            if (ReferenceEquals(this, obj))
                return true;

            // 3. CRITICAL FIX: Check Exact Type
            // If 'this' is a User and 'obj' is a License, this returns FALSE.
            if (this.GetType() != obj.GetType())
                return false;

            // 4. Safe Cast & ID Comparison
            // We know types match, so it's safe to cast to BaseEntity<TKey> just to access the Id.
            var other = (BaseEntity<TKey>)obj;

            // 5. Handle "Transient" Entities (New objects not saved to DB yet)
            // If ID is default (0), they are treated as different unless they are the same reference.
            if (EqualityComparer<TKey>.Default.Equals(Id, default(TKey)) ||
                EqualityComparer<TKey>.Default.Equals(other.Id, default(TKey)))
                return false;

            // 6. Compare the IDs
            return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
        }
        public override int GetHashCode()
        {
            // If transient, use reference-based hash (to keep it stable in HashSets)
            if (IsTransient()) return base.GetHashCode();

            // Otherwise, hash the ID and the Type to prevent collisions between User(5) and License(5)
            return HashCode.Combine(Id, this.GetType());
        }

        public static bool operator ==(BaseEntity<TKey> a, BaseEntity<TKey> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity<TKey> a, BaseEntity<TKey> b)
        {
            return !(a == b);
        }

        // Helper to check if the object is new (not saved to DB)
        private bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
        }
    }
}
