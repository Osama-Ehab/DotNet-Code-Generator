using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    public interface INotifyItemSelected
    {
        // The Notify that fires when the user makes a selection
        event EventHandler<IIdentifiableDto> ItemSelected;
    }
}
