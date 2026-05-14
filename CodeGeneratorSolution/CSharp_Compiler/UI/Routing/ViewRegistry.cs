using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Controls;
using CodeGeneratorSolution.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeGeneratorSolution.UI.Routing
{
    public class ViewRegistry
    {
        // Maps DTO Type -> Details Control Type
        public Dictionary<Type, Type> DetailsRoutes { get; } = new();
        
        // Maps DTO Type -> Editor Control Type
        public Dictionary<Type, Type> EditorRoutes { get; } = new();

        // Helper method to register routes cleanly
        public void RegisterDetails<TReadDTO, TControl>() where TControl : IDetailsControl
        {
            DetailsRoutes[typeof(TReadDTO)] = typeof(TControl);
        }

        public void RegisterEditor<TReadDTO, TControl>() where TControl : IEditorControl
        {
            EditorRoutes[typeof(TReadDTO)] = typeof(TControl);
        }
    }
}