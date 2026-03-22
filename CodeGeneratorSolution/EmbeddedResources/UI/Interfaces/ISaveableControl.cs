using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Interfaces
{
    public interface ISaveableControl : IPopupBase
    {
        int? SavedEntityId { get; }
        // The Form calls this when "Save" is clicked.
        // Returns TRUE if saved successfully, FALSE if validation failed.
        Task<bool> SaveDataAsync();
        // Inside ISaveableControl
        string ActionButtonText { get; } // e.g., "Save", "Update", "Register"
        bool IsReadOnly { get; }         // Tells the form to hide the Save button entirely


        // ADD THESE: Optional Header and Footer controls
        UserControl HeaderControl { get; }
        UserControl BottomControl { get; }

        // ==========================================
        // SMART FORM EVENTS
        // ==========================================

        // Fired when the control finishes fetching data (so the form can update its Title)
        event EventHandler OnDataLoaded;

        // Fired when the control's internal validation passes or fails (Enables/Disables Save button)
        event EventHandler<bool> OnValidationStateChanged;

        // Fired if the control needs to forcefully close the parent form
        event EventHandler OnRequestFormClose;
    }
}
