using {{TARGET_NAMESPACE}}.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    public interface INotifyEdit : INotify
    {
        event EventHandler<DtoEventArgs> Edit;
    }
}
