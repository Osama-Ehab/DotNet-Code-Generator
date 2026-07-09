using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;

namespace {{TARGET_NAMESPACE}}.UI.Interfaces.Controls
{
    public interface IFillControl
    {
        void FillControl(IDto dto);
    }
}
