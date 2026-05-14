using CodeGeneratorSolution.Core.DTOs.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.UI.Interfaces;
using CodeGeneratorSolution.UI.Interfaces.Controls;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.UI.Interfaces
{
    // T represents what you are passing (e.g., an int ID, or a full DTO)
    public interface ISelectorControl<IIdentifiableDto> : IIdentifiableControl,INotifyItemSelected
    {
       

    }
}