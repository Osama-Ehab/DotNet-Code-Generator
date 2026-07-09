using CodeGeneratorSolution.UI.Events;
using CodeGeneratorSolution.UI.Interfaces;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGeneratorSolution.UI.GenericForms
{
    public partial class frmGenericManage : Form
    {
        private readonly IListControlWithNotifies _listControl;
        private readonly INavigationService _navService;

        public frmGenericManage(IListControlWithNotifies listControl, INavigationService navService)
        {
            InitializeComponent();
            _listControl = listControl ?? throw new ArgumentNullException(nameof(listControl));
            _navService = navService ?? throw new ArgumentNullException(nameof(navService));

            // ... (نفس كود التصميم وإضافة الـ UserControl السابق) ...

            // ربط الأحداث بأمان
            this.Load += frmManage_Load;
            this.FormClosed += FrmManage_FormClosed;

            _listControl.AddNew += CtrlList_OnAddNew;
            _listControl.Edit += CtrlList_OnEdit;
            _listControl.ShowDetails += CtrlList_OnShowDetails;
            _listControl.DeleteRequest += CtrlList_OnDeleteRequest; // ربط حدث طلب الحذف
        }

        private void frmManage_Load(object sender, EventArgs e)
        {
            _listControl.LoadDataAsync();
        }
        private void FrmManage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // تنظيف الذاكرة
            _listControl.AddNew -= CtrlList_OnAddNew;
            _listControl.Edit -= CtrlList_OnEdit;
            _listControl.ShowDetails -= CtrlList_OnShowDetails;
            _listControl.DeleteRequest -= CtrlList_OnDeleteRequest;
        }

        // ====================================================
        // EVENT HANDLERS (The Smart Bridge)
        // ====================================================

        private async void CtrlList_OnAddNew(object sender, EventArgs e)
        {
            try
            {
                // 1. ننتظر النتيجة من شاشة الإضافة
                bool hasChanges = await _navService.OpenAddNewAsync(_listControl.EntityDtoType);

                // 2. تحديث مشروط: لا نرهق السيرفر إلا إذا قام المستخدم بالحفظ فعلاً
                if (hasChanges)
                {
                    await _listControl.LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CtrlList_OnEdit(object sender, DtoEventArgs e)
        {
            if (e.SelectedDto == null) return;

            try
            {
                // 1. ننتظر النتيجة من شاشة التعديل
                bool hasChanges = await _navService.OpenAddEditAsync(e.SelectedDto);

                // 2. التحديث المشروط
                if (hasChanges)
                {
                    await _listControl.LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CtrlList_OnShowDetails(object sender, DtoEventArgs e)
        {
            if (e.SelectedDto == null) return;

            try
            {
                // لا نحتاج لـ bool هنا لأن شاشة التفاصيل للقراءة فقط ولا تغير البيانات
                await _navService.ShowDetailsAsync(e.SelectedDto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================
        // THE DELETE LOGIC
        // ====================================================
        private async void CtrlList_OnDeleteRequest(object sender, DtoEventArgs e)
        {
            if (e.SelectedDto == null) return;

            // 1. إظهار رسالة تأكيد موحدة على مستوى النظام كله
            // (يمكنك لاحقاً استبدال هذا بـ ModernMessageBox خاص بك)
            var confirmResult = MessageBox.Show(
                "هل أنت متأكد من أنك تريد حذف هذا السجل بشكل نهائي؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading); // دعم العربية

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // 2. إصدار أمر للأداة (UserControl) لتقوم هي بحذف السجل من قاعدة البيانات
                    bool deleteSuccess = await _listControl.DeleteDataAsync(e.SelectedDto);

                    // 3. إذا نجح الحذف داخلياً، نقوم بتحديث الشبكة (الـ Grid)
                    if (deleteSuccess)
                    {
                        await _listControl.LoadDataAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}