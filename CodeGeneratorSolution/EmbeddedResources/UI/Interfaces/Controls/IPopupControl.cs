using {{TARGET_NAMESPACE}}.UI.Interfaces;
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
    public interface IPopupControl : IPopup, IUserControl
    {
        
    }
}
