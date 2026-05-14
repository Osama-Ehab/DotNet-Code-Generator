using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.Core.Entities.Interfaces
{
    public interface IEntity<Tkey>
    {
        public Tkey Id { get; set; }
    }
}
