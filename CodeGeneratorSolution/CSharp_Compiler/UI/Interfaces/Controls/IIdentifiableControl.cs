using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Events;
using CodeGeneratorSolution.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.UI.Interfaces.Controls
{
    public interface IIdentifiableControl : IPopupControl, ILoadDataById, INotifyDataLoaded
    {
    }
}
