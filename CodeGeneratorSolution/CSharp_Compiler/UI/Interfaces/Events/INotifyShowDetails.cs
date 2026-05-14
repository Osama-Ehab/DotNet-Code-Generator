using CodeGeneratorSolution.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces
{
    public interface INotifyShowDetails : INotify
    {
        event EventHandler<DtoEventArgs> ShowDetails;
    }
}
