using CodeGeneratorSolution.EmbeddedResources.Core.DTOs.Interfaces;
using CodeGeneratorSolution.EmbeddedResources.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Core.DTOs
{
    public abstract class BaseDtoWithId<TKey> : IBaseDtoWithId<TKey>
    {
        public required TKey Id { get; set; }
    }
}
