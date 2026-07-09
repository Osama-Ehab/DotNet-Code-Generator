using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernUI.Framework.Interfaces
{
    public interface IHoverableIconControl : IIconableControl
    {
        string IconHoverText { get; set; }
        Color IconHoverColor { get; set; }
    }
}
