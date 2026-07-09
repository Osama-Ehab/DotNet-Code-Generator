
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Core.DTOs.Interfaces
{
    // 1. The Root Marker (For the Router to identify ANY DTO)
    // It has NO properties. It just says "I am a DTO".
    public interface IDto
    {
    }

    // 2. The Non-Generic Identity (The Router's Back Door)
    // Only applied to DTOs that actually HAVE an ID from the database.
    public interface IIdentifiableDto : IDto
    {
        object Id { get; }
    }

    // 3. The Generic Identity (The Developer's Front Door)
    // Provides strict type safety for Lists, Details, and Updates.
    public interface IIdentifiableDto<TKey> : IIdentifiableDto
    {
        new TKey Id { get; set; }
    }
}
