using CodeGeneratorSolution.Core.DTOs.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Events;
using CodeGeneratorSolution.UI.Interfaces.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.UI.Interfaces
{
    public interface IEditorControl<Tkey> : IEditorControl
    {
        Tkey? SavedEntityId { get; }
        // The Form calls this when "Save" is clicked.
        // Returns TRUE if saved successfully, FALSE if validation failed.
    }
    public interface IEditorControl :IIdentifiableControl
    {
        
        Task<bool> SaveDataAsync();
        // Inside IEditorControl
        string ActionButtonText { get; } // e.g., "Save", "Update", "Register"
        bool IsReadOnly { get; }         // Tells the form to hide the Save button entirely


        // ADD THESE: Optional Header and Footer controls
        UserControl HeaderControl { get; }
        UserControl BottomControl { get; }

        // ==========================================
        // SMART FORM EVENTS
        // ==========================================



        // Fired when the control's internal validation passes or fails (Enables/Disables Save button)
        //event EventHandler<bool> OnValidationStateChanged;

        // Fired if the control needs to forcefully close the parent form
        //event EventHandler OnRequestFormClose;
    }
}
