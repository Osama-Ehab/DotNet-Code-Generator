
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    // T represents what you are passing (e.g., an int ID, or a full DTO)
    public interface ISelectorControl<IIdentifiableDto> : IIdentifiableControl, INotifyItemSelected
    {
       

    }
}