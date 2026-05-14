using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces
{

    public interface ILoadData
    {
        // The standard method every generated control must implement
        Task LoadDataAsync();
    }
    public interface ILoadDataById
    {
        // The standard method every generated control must implement
        Task LoadDataAsync(object Id);
    }

}
