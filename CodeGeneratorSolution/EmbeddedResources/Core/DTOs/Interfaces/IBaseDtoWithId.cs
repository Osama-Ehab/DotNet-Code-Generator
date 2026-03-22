using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Core.DTOs.Interfaces
{
    internal interface IBaseDtoWithId<TKey> : IBaseDto
    {
        TKey Id { get; }
    }
}
