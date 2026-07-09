using ModernUI.Framework.Controls; // مسار الأداة الجديدة
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Controls.BaseControls
{
    // 1. إزالة abstract
    // 2. إخفاء الكنترول من الـ Toolbox حتى لا يسحبه أحد بالخطأ
    [ToolboxItem(false)]
    public partial class ShowDetailsBaseControl : UserControl
    {
        // نجعل الجدول protected لكي تتمكن الشاشات الوارثة من رؤيته إذا لزم الأمر
        protected TableLayoutPanel _tlpDetails;

        public ShowDetailsBaseControl()
        {
            //InitializeComponent();

            if (this.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            SetupLayout();
        }

        private void SetupLayout()
        {
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.AutoScroll = true;

            // جدول واحد مسطح (Flat Table)
            _tlpDetails = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                RightToLeft = RightToLeft.Yes, // أو اربطه بـ AppLayoutContext
                Margin = new Padding(0)
            };

            this.Controls.Add(_tlpDetails);
        }

        public void BeginDataUpdate(int totalColumnsCount)
        {
            _tlpDetails.SuspendLayout();
            _tlpDetails.Controls.Clear();
            _tlpDetails.ColumnStyles.Clear();
            _tlpDetails.RowStyles.Clear();

            _tlpDetails.ColumnCount = totalColumnsCount;
            float percent = 100f / totalColumnsCount;
            for (int i = 0; i < totalColumnsCount; i++)
            {
                _tlpDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
            }
            _tlpDetails.RowCount = 0;
        }

        // الدالة الآن تُرجع ModernDisplayField للمبرمج
        public ModernDisplayField AddDetailField(string iconCode, string displayName, string stringValue, int uiRow, int uiColumn, int uiColSpan)
        {
            while (_tlpDetails.RowCount <= uiRow)
            {
                _tlpDetails.RowCount++;
                _tlpDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            // الاعتماد على الأداة الخفيفة والمبنية بـ GDI+
            ModernDisplayField field = new ModernDisplayField
            {
                IconText = iconCode,
                LabelText = displayName,
                Value = stringValue ?? "غير متوفر",
                Dock = DockStyle.Fill,
                IsArabicLayout = (_tlpDetails.RightToLeft == RightToLeft.Yes)
            };

            // وضعها في الشبكة
            _tlpDetails.Controls.Add(field, uiColumn, uiRow);
            _tlpDetails.SetColumnSpan(field, uiColSpan);

            return field;
        }

        public void EndDataUpdate()
        {
            _tlpDetails.ResumeLayout();
        }
    }
}