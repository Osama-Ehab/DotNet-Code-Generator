using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{TARGET_NAMESPACE}}.UI.Events;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    public interface INotifyShowDetails : INotify
    {
    event EventHandler<DtoEventArgs> ShowDetails;
}
}
