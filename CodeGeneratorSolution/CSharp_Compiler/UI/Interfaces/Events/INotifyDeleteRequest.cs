using CodeGeneratorSolution.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Events
{
    // 2. واجهة طلب الحذف (الحدث الذي تطلقه الأداة)
    public interface INotifyDeleteRequest : INotify
    {
        event EventHandler<DtoEventArgs> DeleteRequest;
    }
}
