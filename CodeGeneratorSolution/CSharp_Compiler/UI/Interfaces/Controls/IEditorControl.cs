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
    public interface IEditorControl : IIdentifiableControl
    {
        Task<bool> SaveDataAsync();
        string ActionButtonText { get; }
        bool IsReadOnly { get; }

        // REMOVED: HeaderControl and BottomControl
        // We don't need them anymore! The control itself handles its UI.
    }
}
