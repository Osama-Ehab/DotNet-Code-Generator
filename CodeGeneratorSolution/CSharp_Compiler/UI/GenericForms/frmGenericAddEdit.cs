
using CodeGeneratorSolution.UI.Helpers;
using CodeGeneratorSolution.UI.Interfaces;
using ModernUI.Framework.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGeneratorSolution.UI.GenericForms
{
    public partial class frmGenericAddEdit : Form
    {
        private readonly IEditorControl _EditorContent;

        // Hold a class-level reference to the Layout Engine
        private readonly TableLayoutPanel _mainLayout;

        public frmGenericAddEdit(IEditorControl Editor)
        {
            InitializeComponent();
            _EditorContent = Editor;

            this.Text = _EditorContent.Title;
            this.AutoSize = true;
            // ...

            // The Form layout becomes incredibly simple: Just Content and Buttons
            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2, // 0: The ENTIRE Control, 1: Buttons
                AutoSize = true
            };

            _mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            this.Controls.Add(_mainLayout);

            // 1. Add the Unified Control
            var ctrl = Editor.AsUserControl();
            ctrl.Dock = DockStyle.Fill;
            _mainLayout.Controls.Add(ctrl, 0, 0);

            // 2. Add Buttons
            var btnPanel = CreateButtonPanel(ctrl.Width);
            _mainLayout.Controls.Add(btnPanel, 0, 1);

            SubscribeToSmartEvents();
            this.FormClosed += FrmGenericAddEdit_FormClosed;
        }
        private void SubscribeToSmartEvents()
        {
            if (_EditorContent != null)
            {
                _EditorContent.DataLoaded += HandleDataLoaded;
               // _EditorContent.RequestFormClose += HandleRequestFormClose;
            }
        }

        private void FrmGenericAddEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // MEMORY LEAK PREVENTION: Unsubscribe from all events!
            if (_EditorContent != null)
            {
                _EditorContent.DataLoaded -= HandleDataLoaded;
                //_EditorContent.OnRequestFormClose -= HandleRequestFormClose;
            }
        }

        // ==========================================
        // EVENT HANDLERS (The "Smart" Logic)
        // ==========================================

        private void HandleDataLoaded(object sender, EventArgs e)
        {
            // Update the form's title dynamically!
            // e.g., Changes from "Loading..." to "Edit User: admin123"
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Text = _EditorContent.Title));
            }
            else
            {
                this.Text = _EditorContent.Title;
            }
        }

        private void HandleRequestFormClose(object sender, EventArgs e)
        {
            // Allow the UserControl to close the form (e.g., if a record was deleted internally)
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        public void AddBottomControl(UserControl bottomCtrl)
        {
            bottomCtrl.Dock = DockStyle.Fill;
            bottomCtrl.Margin = new Padding(0, 0, 0, 15);

            // Drop it exactly into Column 0, Row 2.
            _mainLayout.Controls.Add(bottomCtrl, 0, 2);
        }

        private Panel CreateButtonPanel(int formWidth)
        {
            // 1. الحاوية الرئيسية السفلية (Panel العادي)
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            // 2. حاوية التدفق (FlowLayoutPanel) لرص الأزرار بمرونة
            FlowLayoutPanel flowLayout = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft, // يبدأ الرص من اليمين لليسار
                WrapContents = false,
                Location = new Point(formWidth - 10, 5), // إزاحة مبدئية
                Anchor = AnchorStyles.Top | AnchorStyles.Right, // السحر هنا: يلتصق باليمين دائماً
                Padding = new Padding(0)
            };

            // 3. إضافة زر الإلغاء (Danger Button)
            // بافتراض أنك ستقوم ببرمجة ModernButton لاحقاً
            DangerButton btnCancel = new DangerButton
            {
                Text = "إلغاء",
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(5, 0, 0, 0) // مسافة بين الأزرار
            };
            btnCancel.Click += (s, e) => this.Close();
            this.CancelButton = btnCancel;

            // 4. إضافة زر الحفظ (Success Button)
            if (!_EditorContent.IsReadOnly)
            {
                SuccessButton btnSave = new SuccessButton
                {
                    Text = string.IsNullOrEmpty(_EditorContent.ActionButtonText) ? "حفظ" : _EditorContent.ActionButtonText,
                    DialogResult = DialogResult.None,
                    Margin = new Padding(5, 0, 0, 0)
                };
                btnSave.Click += btnSave_Click;
                this.AcceptButton = btnSave;

                // إضافته أولاً ليكون في أقصى اليمين (بسبب RightToLeft)
                flowLayout.Controls.Add(btnSave);
            }

            // إضافة زر الإلغاء ثانياً
            flowLayout.Controls.Add(btnCancel);

            // ربط الحاويات
            pnlBottom.Controls.Add(flowLayout);

            // تعديل موقع الـ FlowLayout بناءً على عرضه النهائي بعد رص الأزرار
            flowLayout.Left = formWidth - flowLayout.Width - 15;

            return pnlBottom;
        }
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                return;

            var btn = (Button)sender;
            btn.Enabled = false;

            try
            {
                bool success = await _EditorContent.SaveDataAsync();

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected UI/Form-level errors
                MessageServiceHelper.ShowError($"An error occurred while saving: {ex.Message}");
            }
            finally
            {
                // Safety check in case the form is already disposed
                if (!this.IsDisposed && !btn.IsDisposed)
                {
                    btn.Enabled = true;
                }
            }
        }
    }
}
