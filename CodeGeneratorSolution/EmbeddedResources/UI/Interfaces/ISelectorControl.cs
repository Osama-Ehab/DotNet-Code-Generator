using CodeGeneratorSolution.EmbeddedResources.Core.DTOs.Interfaces;
using CodeGeneratorSolution.EmbeddedResources.UI.Interfaces;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.UI.Interfaces
{
    // T represents what you are passing (e.g., an int ID, or a full DTO)
    public interface ISelectorControl<IBaseDto> : IPopupBase
    {
        // The event that fires when the user makes a selection
        event EventHandler<IBaseDto> OnItemSelected;

    }
}