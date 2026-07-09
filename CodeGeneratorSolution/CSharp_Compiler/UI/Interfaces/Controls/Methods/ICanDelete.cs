using CodeGeneratorSolution.Core.DTOs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Controls.Methods
{
    // 3. واجهة تنفيذ الحذف (لكي يتمكن الفورم من أمر الأداة بالحذف)
    public interface ICanDelete
    {
        Task<bool> DeleteDataAsync(IIdentifiableDto dto);
    }
}
