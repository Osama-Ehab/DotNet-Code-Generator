
using CodeGeneratorSolution.Application.Interfaces;
using CodeGeneratorSolution.Core.DTOs.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Controls;
using CodeGeneratorSolution.UI.Events;
using CodeGeneratorSolution.UI.GenericForms;
using CodeGeneratorSolution.UI.Helpers;
using CodeGeneratorSolution.UI.Interfaces;
using CodeGeneratorSolution.UI.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGeneratorSolution.UI.Services
{

    public class WindowsFormsNavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ViewRegistry _registry;

        public WindowsFormsNavigationService(IServiceProvider serviceProvider, ViewRegistry registry)
        {
            _serviceProvider = serviceProvider;
            _registry = registry;
        }

        // ====================================================
        // 1. DYNAMIC ROUTING FOR SHOW DETAILS (O(1) Lookup)
        // ====================================================
        public async Task ShowDetailsAsync(IIdentifiableDto dto)
        {
            Type dtoType = dto.GetType();

            // 1. Find the target UserControl type in the map
            if (!_registry.DetailsRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"No details view configured for {dtoType.Name}.", "Routing Error");
                return;
            }

            // 2. Resolve and Show
            using (var scope = _serviceProvider.CreateScope())
            {
                // Magic: GetRequiredService accepts a Type variable!
                var control = (IDetailsControl)scope.ServiceProvider.GetRequiredService(controlType);

                await control.LoadDataAsync(dto.Id);

                using (var frm = new frmGenericPopup(control))
                {
                    frm.ShowDialog();
                }
            }
        }

        // ====================================================
        // 2. DYNAMIC ROUTING FOR UPDATES (O(1) Lookup)
        // ====================================================
        public async Task OpenAddEdit(IIdentifiableDto gridDto)
        {
            if (gridDto == null) return;
            Type dtoType = gridDto.GetType();

            if (!_registry.EditorRoutes.TryGetValue(dtoType, out Type controlType)) return;

            using (var scope = _serviceProvider.CreateScope())
            {
                var control = (IEditorControl)scope.ServiceProvider.GetRequiredService(controlType);

                // handle async properly depending on your UI flow
                await control.LoadDataAsync(gridDto.Id);

                using (var frm = new frmGenericAddEdit(control))
                {
                    frm.ShowDialog();
                }
            }
        }


        // ====================================================
        // 3. GENERIC ADD NEW RECORD (No Code Duplication!)
        // ====================================================

        public void OpenAddNew(Type dtoType)
        {
            // 1. O(1) Hash Lookup - Instantaneous!
            if (_registry.EditorRoutes.TryGetValue(dtoType, out Type editorType))
            {

                using (var scope = _serviceProvider.CreateScope())
                {
                    // 2. Pass the resolved Type to the DI Container!
                    var control = (IEditorControl)scope.ServiceProvider.GetRequiredService(editorType);

                    using (var frm = new frmGenericAddEdit(control))
                    {
                        frm.ShowDialog();
                    }
                }
            }
            else
            {
                MessageServiceHelper.ShowError($"Routing failed: No editor mapped for {dtoType.Name}");
            }
        }
    }
}
