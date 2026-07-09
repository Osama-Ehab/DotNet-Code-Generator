using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeGeneratorSolution.UI.Interfaces;
using ModernUI.Framework.Controls;

namespace CodeGeneratorSolution.UI.GenericForms
{
    public partial class frmGenericPopup : Form
    {
        // ==========================================
        // 1. Constructor Chaining (تطبيق مبدأ DRY)
        // ==========================================
        // إذا تم إرسال عنصر واحد، نقوم بتحويله إلى List ونرسله للـ Constructor الأساسي
        public frmGenericPopup(IPopupControl contentItem)
            : this(new List<IPopupControl> { contentItem })
        {
        }

        // الـ Constructor الأساسي الذي يقوم بكل العمل
        public frmGenericPopup(List<IPopupControl> contentItems)
        {
            InitializeComponent();

            if (contentItems == null || !contentItems.Any())
                throw new ArgumentException("يجب توفير أداة واحدة على الأقل لعرضها في النافذة.");

            // 1. إعدادات الفورم (Form Setup)
            this.Text = contentItems.First().Title ?? "Details";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White; // توحيد اللون مع الإطار العصري
            this.Padding = new Padding(5);

            // 2. محرك التخطيط الأساسي (TableLayoutPanel)
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = contentItems.Count + 1, // صف لكل أداة + صف للأزرار في الأسفل
                Margin = new Padding(0)
            };

            this.Controls.Add(mainLayout);

            // 3. إضافة المحتوى (الأدوات)
            int currentRow = 0;
            int maxCalculatedWidth = 300;

            foreach (var item in contentItems)
            {
                // تحديث أقصى عرض مرغوب
                if (item.Width > maxCalculatedWidth) maxCalculatedWidth = item.Width;

                var ctrl = item.AsUserControl();
                ctrl.Dock = DockStyle.Fill; // لضمان تمدد الأداة أفقياً داخل الصف
                ctrl.Margin = new Padding(5, 5, 5, 10);

                // منع انكماش الأداة أقل من عرضها التصميمي
                ctrl.MinimumSize = new Size(item.Width, ctrl.MinimumSize.Height);

                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                mainLayout.Controls.Add(ctrl, 0, currentRow);

                currentRow++;
            }

            // 4. إضافة لوحة الأزرار في الصف الأخير
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // صف ثابت الارتفاع للأزرار
            mainLayout.Controls.Add(CreateFooterPanel(maxCalculatedWidth), 0, currentRow);
        }

        // ==========================================
        // 2. محرك توليد الأزرار (Dynamic Footer)
        // ==========================================
        private Panel CreateFooterPanel(int formWidth)
        {
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            // نستخدم FlowLayoutPanel لرص الأزرار بذكاء وتجنب الحسابات اليدوية الصعبة
            FlowLayoutPanel flowLayout = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right, // الالتصاق بالزاوية
                Location = new Point(formWidth - 100, 10) // موقع مبدئي، سيتم تحديثه تلقائياً
            };

            // زر الإغلاق (يمكنك تغييره لاحقاً إلى ModernButton من نوع Danger أو Neutral)
            DangerButton btnClose = new DangerButton
            {
                Text = "إغلاق",
                Size = new Size(90, 32),
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(5, 0, 0, 0)
            };

            btnClose.Click += (s, e) => this.Close();

            flowLayout.Controls.Add(btnClose);
            pnlFooter.Controls.Add(flowLayout);

            // تعديل الموقع الفعلي بعد حساب الحجم
            flowLayout.Left = formWidth - flowLayout.Width - 5;

            // ربط زر الإلغاء بزر الهروب (ESC) في الكيبورد
            this.CancelButton = btnClose;

            return pnlFooter;
        }
    }
}