using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernUI.Framework.Interfaces
{
    public interface IFocusableIconControl : IIconableControl
    {
        string IconFocusedText { get; set; }
        Color IconFocusedColor { get; set; }
    }
}
