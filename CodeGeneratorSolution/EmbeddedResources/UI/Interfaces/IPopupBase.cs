using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Interfaces
{
    // ---------------------------------------------------------
    // TIER 1: The "Visual" Interface (Non-Generic)
    // The Generic Form uses THIS one. It doesn't care about IDs.
    // ---------------------------------------------------------
    public interface IPopupBase
    {
        // 1. Metadata: The control tells the form what the title should be
        string WindowTitle { get; }

        // 2. Size: Optional, if you want the control to dictate width
        int PreferredWidth { get; }

        // 3. Helper to easily get the UI component (cast to Control)
        UserControl AsUserControl();
    }
}
