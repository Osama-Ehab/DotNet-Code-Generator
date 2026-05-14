using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGeneratorSolution.Core.DTOs.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;



namespace CodeGeneratorSolution.UI.Interfaces
{
    public interface INavigationService : IShowDetails,IOpenAddEdit,IOpenAddNew
    {
     
    }

}
