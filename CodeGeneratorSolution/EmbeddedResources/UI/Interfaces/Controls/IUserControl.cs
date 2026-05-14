using {{TARGET_NAMESPACE}}.UI.Interfaces.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    public interface IUserControl : IFillControl, ITitle,IWidth
    {
        //  Helper to easily get the UI component (cast to Control)
        UserControl AsUserControl();
    }
}
