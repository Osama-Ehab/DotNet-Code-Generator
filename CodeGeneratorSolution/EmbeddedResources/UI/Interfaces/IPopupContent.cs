using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Interfaces
{
    // ---------------------------------------------------------
    // TIER 2: The "Functional" Interface (Generic)
    // The Navigation Service uses THIS one to load data.
    // TKey allows us to handle int, long, Guid, etc.
    // ---------------------------------------------------------
    public interface IPopupContent<TKey> : IPopupBase
    {
        // The standard method every generated control must implement
         Task LoadDataAsync(TKey id);
    }
}
