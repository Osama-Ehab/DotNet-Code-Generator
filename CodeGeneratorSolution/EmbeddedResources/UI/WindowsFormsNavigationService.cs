
using {{TARGET_NAMESPACE}}.Application.Interfaces;
using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Interfaces.Controls;
using {{TARGET_NAMESPACE}}.UI.Events;
using {{TARGET_NAMESPACE}}.UI.GenericForms;
using {{TARGET_NAMESPACE}}.UI.Helpers;
using {{TARGET_NAMESPACE}}.UI.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace {{TARGET_NAMESPACE}}.UI.Services
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
        // 1. SHOW DETAILS 
        // ====================================================
        public async Task ShowDetailsAsync(IIdentifiableDto dto)
        {
            Type dtoType = dto.GetType();

            if (!_registry.DetailsRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"No details view configured for {dtoType.Name}.", "Routing Error");
                return;
            }

            await OpenRoutedControlAsync<IDetailsControl>(controlType, true, async (control) =>
            {
                await control.LoadDataAsync(dto.Id);
            });
        }

        // ====================================================
        // 2. OPEN ADD/EDIT 
        // ====================================================
        public async Task OpenAddEdit(IIdentifiableDto gridDto)
        {
            if (gridDto == null) return;
            Type dtoType = gridDto.GetType();

            if (!_registry.EditorRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"Routing failed: No editor mapped for {dtoType.Name}", "Routing Error");
                return;
            }

            await OpenRoutedControlAsync<IEditorControl>(controlType, false, async (control) =>
            {
                await control.LoadDataAsync(gridDto.Id);
            });
        }

        // ====================================================
        // 3. OPEN ADD NEW 
        // ====================================================
        public async Task OpenAddNew(Type dtoType)
        {
            if (!_registry.EditorRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"Routing failed: No editor mapped for {dtoType.Name}", "Routing Error");
                return;
            }

            await OpenRoutedControlAsync<IEditorControl>(controlType, false, null);
        }

        // ====================================================
        // 💎 THE CORE ENGINE (DRY Principle)
        // ====================================================
        /// <summary>
        /// المحرك الأساسي لفتح الشاشات: يعالج الـ DI، والـ Scopes، ومؤشر الانتظار، والأخطاء.
        /// </summary>
        private async Task OpenRoutedControlAsync<TControlInterface>(Type controlType, bool isDetails, Func<TControlInterface, Task> loadDataAction)
        {
            // تغيير شكل الماوس لحالة الانتظار لكي لا يظن المستخدم أن النظام معطل
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // 1. محاولة استخراج الشاشة من الـ DI
                    // استخدام GetService بدلاً من GetRequiredService لكي نمسك الخطأ بأناقة
                    var resolvedService = scope.ServiceProvider.GetService(controlType);

                    if (resolvedService == null)
                    {
                        MessageServiceHelper.ShowError($"The control '{controlType.Name}' is routed, but it is NOT registered in the Dependency Injection container (Program.cs).", "DI Registration Missing");
                        return;
                    }

                    var control = (TControlInterface)resolvedService;

                    // 2. تحميل البيانات (إن وجدت) أثناء ظهور مؤشر الانتظار
                    if (loadDataAction != null)
                    {
                        await loadDataAction(control);
                    }

                    // إعادة الماوس لطبيعته قبل فتح الشاشة
                    Cursor.Current = Cursors.Default;

                    // 3. تغليف الشاشة وعرضها
                    Form frm = isDetails
                        ? new frmGenericPopup((IDetailsControl)control)
                        : new frmGenericAddEdit((IEditorControl)control);

                    using (frm)
                    {
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageServiceHelper.ShowError($"A critical error occurred while opening the screen:\n\n{ex.Message}", "System Error");
            }
        }
    }
}
