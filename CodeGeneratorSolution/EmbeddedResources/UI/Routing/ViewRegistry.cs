using {{TARGET_NAMESPACE}}.UI.Interfaces.Controls;
using {{TARGET_NAMESPACE}}.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace {{TARGET_NAMESPACE}}.UI.Routing
{
    public partial class ViewRegistry
    {
        // Maps DTO Type -> Details Control Type
        public Dictionary<Type, Type> DetailsRoutes { get; } = new();
        
        // Maps DTO Type -> Editor Control Type
        public Dictionary<Type, Type> EditorRoutes { get; } = new();

        // Helper method to register routes cleanly
        public void RegisterDetails<TListDTO, TControl>() where TControl : IDetailsControl
        {
            DetailsRoutes[typeof(TListDTO)] = typeof(TControl);
        }

        public void RegisterEditor<TListDTO, TControl>() where TControl : IEditorControl
        {
            EditorRoutes[typeof(TListDTO)] = typeof(TControl);
        }
    }
}