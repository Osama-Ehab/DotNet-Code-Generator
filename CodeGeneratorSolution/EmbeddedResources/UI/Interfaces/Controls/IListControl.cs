using {{TARGET_NAMESPACE}}.UI.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    // ---------------------------------------------------------
    // TIER 2: The "Functional" Interface (Generic)
    // The Navigation Service uses THIS one to load data.
    // TKey allows us to handle int, long, Guid, etc.
    // ---------------------------------------------------------
     public interface IListControl : IPopupControl, IEntityDtoType,ILoadData
{

}
public interface IListControlWithNotifies : IListControl, INotifyAddNew, INotifyEdit, INotifyShowDetails
{

}


}
