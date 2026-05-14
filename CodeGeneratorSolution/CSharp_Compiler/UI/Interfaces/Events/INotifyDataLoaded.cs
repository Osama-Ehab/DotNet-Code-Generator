using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Events
{
    public interface INotifyDataLoaded
    {
        // Fired when the control finishes fetching data (so the form can update its Title)
        event EventHandler DataLoaded;
    }
}
