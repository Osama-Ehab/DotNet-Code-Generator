using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernUI.Framework.Interfaces
{
    public interface IIconableControl : IModernControl
    {
        string IconText { get; set; }
        Color IconColor { get; set; }
    }
}
