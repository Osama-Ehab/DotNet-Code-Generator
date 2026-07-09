using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using ModernUI.Framework.Controls; // مسار الأداة الجديدة

namespace {{TARGET_NAMESPACE}}.UI.Controls
{
    public partial class ShowDetailsBaseControl : UserControl
    {
    protected TableLayoutPanel _tlpDetails;

    // 💎 إضافة الكنترول المرسوم بـ GDI+
    protected ModernControlHeader _mainHeader;

    // ==========================================
    // خاصية التحكم بظهور الترويسة (ستظهر في شاشة الخصائص)
    // ==========================================
    [Category("Modern UI")]
    [Description("إظهار أو إخفاء الترويسة العلوية للشاشة.")]
    [DefaultValue(true)]
    public bool HeaderVisible
    {
        get => _mainHeader != null && _mainHeader.Visible;
        set
        {
            if (_mainHeader != null) _mainHeader.Visible = value;
        }
    }

    public ShowDetailsBaseControl()
    {
        InitializeComponent();
        if (this.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
        SetupLayout();
    }

    private void SetupLayout()
    {
        this.BackColor = Color.White;
        this.Padding = new Padding(15);
        this.AutoScroll = true;

        // 1. إنشاء جدول التفاصيل (يأخذ المساحة المتبقية)
        _tlpDetails = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            RightToLeft = RightToLeft.Yes,
            Margin = new Padding(0)
        };
        this.Controls.Add(_tlpDetails);

        // 2. إنشاء الـ Header العبقري الخاص بك
        _mainHeader = new ModernControlHeader
        {
            Dock = DockStyle.Top,
            Height = 85, // ارتفاع مناسب للأيقونة والنص
            Visible = true
        };
        this.Controls.Add(_mainHeader);

        // ⚠️ ملاحظة مهمة للـ Z-Order في WinForms:
        // لكي يلتصق الـ Header بأعلى الشاشة فوق الجدول، يجب إرساله للمقدمة
        _mainHeader.BringToFront();
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