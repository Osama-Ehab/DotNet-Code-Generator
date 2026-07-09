using CodeGeneratorSolution.Core.DTOs.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Controls;
using CodeGeneratorSolution.UI.GenericForms;
using CodeGeneratorSolution.UI.Helpers;
using CodeGeneratorSolution.UI.Interfaces;
using CodeGeneratorSolution.UI.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        // 1. SHOW DETAILS (لا يرجع بيانات لأنها للقراءة فقط)
        // ====================================================
        public async Task ShowDetailsAsync(IIdentifiableDto dto)
        {
            Type dtoType = dto.GetType();

            if (!_registry.DetailsRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"No details view configured for {dtoType.Name}.", "Routing Error");
                return;
            }

            await OpenRoutedControlAsync<IPopupControl>(controlType, true, async (control) =>
            {
                // بافتراض أن IDetailsControl يحتوي على LoadDataAsync
                if (control is ILoadDataById loadableControl)
                {
                    await loadableControl.LoadDataAsync(dto.Id);
                }
            });
        }

        // ====================================================
        // 2. OPEN ADD/EDIT (يرجع True إذا تم الحفظ بنجاح)
        // ====================================================
        public async Task<bool> OpenAddEditAsync(IIdentifiableDto gridDto)
        {
            if (gridDto == null) return false;
            Type dtoType = gridDto.GetType();

            if (!_registry.EditorRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"Routing failed: No editor mapped for {dtoType.Name}", "Routing Error");
                return false;
            }

            return await OpenRoutedControlAsync<IEditorControl>(controlType, false, async (control) =>
            {
                if (control is ILoadDataById loadableControl)
                {
                    await loadableControl.LoadDataAsync(gridDto.Id);
                }
            });
        }

        // ====================================================
        // 3. OPEN ADD NEW (يرجع True إذا تم الحفظ بنجاح)
        // ====================================================
        public async Task<bool> OpenAddNewAsync(Type dtoType)
        {
            if (!_registry.EditorRoutes.TryGetValue(dtoType, out Type controlType))
            {
                MessageServiceHelper.ShowError($"Routing failed: No editor mapped for {dtoType.Name}", "Routing Error");
                return false;
            }

            return await OpenRoutedControlAsync<IEditorControl>(controlType, false, null);
        }

        // ====================================================
        // 💎 THE CORE ENGINE (DRY Principle)
        // ====================================================
        /// <summary>
        /// المحرك الأساسي لفتح الشاشات: يعالج الـ DI، الـ Scopes، مؤشر الانتظار، ويرجع حالة الحفظ.
        /// </summary>
        private async Task<bool> OpenRoutedControlAsync<TControlInterface>(Type controlType, bool isDetails, Func<TControlInterface, Task> loadDataAction)
        {
            // تغيير شكل الماوس لحالة الانتظار لكي لا يظن المستخدم أن النظام معطل
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // 1. محاولة استخراج الشاشة من الـ DI
                    var resolvedService = scope.ServiceProvider.GetService(controlType);

                    if (resolvedService == null)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageServiceHelper.ShowError($"The control '{controlType.Name}' is routed, but it is NOT registered in the Dependency Injection container (Program.cs).", "DI Registration Missing");
                        return false;
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
                        ? new frmGenericPopup((IPopupControl)control)
                        : new frmGenericAddEdit((IEditorControl)control);

                    using (frm)
                    {
                        // 4. التقاط النتيجة (DialogResult) لإخبار الـ Grid هل تم التحديث أم لا!
                        DialogResult result = frm.ShowDialog();
                        return result == DialogResult.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageServiceHelper.ShowError($"A critical error occurred while opening the screen:\n\n{ex.Message}", "System Error");
                return false;
            }
        }

   
    }
}