using CodeGeneratorSolution.UI.Events;
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
    public interface IContentList : IPopupBase
    {
        Type EntityDtoType { get; }
        // The standard method every generated control must implement
        Task LoadDataAsync();

        // 4. Decoupled Events
        event EventHandler OnAddNewRequested;
        event EventHandler<DtoEventArgs> OnEditRequested;
        event EventHandler<DtoEventArgs> OnDetailsRequested;
    }
}
