using ModernUI.Framework.Icons;
using ModernUI.Framework.Interfaces;
using ModernUI.Framework.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    [DefaultEvent("ValueChanged")]
    [ToolboxItem(true)]
    public partial class ModernDateTimePicker : ModernControlBase // تمت إزالة IFocusableIconControl
    {
        // =========================================================
        // 2. نظام الأيقونات وتطبيق الواجهة (IFocusableIconControl)
        // =========================================================
        private string _rawIconText = SegoeIcons.Calendar.Outline; // أيقونة افتراضية للبحث/القوائم
        private string _iconText = "";

        private string _rawIconFocusedText = SegoeIcons.Calendar.Solid;
        private string _iconFocusedText = "";

        private Font _iconFont = new Font("Segoe MDL2 Assets", 11F, FontStyle.Regular);
        private Color _iconColor = Color.FromArgb(150, 150, 150);
        private Color _iconFocusedColor = Color.FromArgb(52, 152, 219);

        [Category("Modern UI - Icons")]
        public string IconText
        {
            get => _rawIconText;
            set { _rawIconText = value; _iconText = ModernIconUtility.ParseIconText(value); AdjustLayout(); this.Invalidate(); }
        }

        [Category("Modern UI - Icons")]
        public string IconFocusedText
        {
            get => _rawIconFocusedText;
            set { _rawIconFocusedText = value; _iconFocusedText = ModernIconUtility.ParseIconText(value); AdjustLayout(); this.Invalidate(); }
        }

        [Category("Modern UI - Icons")]
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; this.Invalidate(); }
        }

        [Category("Modern UI - Icons")]
        public Color IconFocusedColor
        {
            get => _iconFocusedColor;
            set { _iconFocusedColor = value; this.Invalidate(); }
        }



        private DateTimePicker _dtpDate;
        public event EventHandler ValueChanged;

        // الخصائص التغليفية للـ DateTimePicker
        [Category("Modern UI - Data")] public DateTime Value { get => _dtpDate.Value; set => _dtpDate.Value = value; }
        [Category("Modern UI - Data")] public DateTimePickerFormat Format { get => _dtpDate.Format; set => _dtpDate.Format = value; }
        [Category("Modern UI - Data")] public string CustomFormat { get => _dtpDate.CustomFormat; set => _dtpDate.CustomFormat = value; }
        [Category("Modern UI - Behavior")] public bool AllowNull { get => _dtpDate.ShowCheckBox; set => _dtpDate.ShowCheckBox = value; }

        [Category("Modern UI - Data")]
        [Browsable(false)]
        public DateTime? NullableValue
        {
            get { return (_dtpDate.ShowCheckBox && !_dtpDate.Checked) ? (DateTime?)null : _dtpDate.Value; }
            set { if (value.HasValue) { _dtpDate.Value = value.Value; _dtpDate.Checked = true; } else { _dtpDate.Value = DateTime.Now; _dtpDate.Checked = false; } }
        }

        public ModernDateTimePicker()
        {
            this.Size = new Size(203, 52); // زيادة الارتفاع الكلي قليلاً

            _dtpDate = new DateTimePicker();
            _dtpDate.Font = new Font("Segoe UI", 10F);
            _dtpDate.Format = DateTimePickerFormat.Short;
            _dtpDate.ShowCheckBox = false;
            

            _dtpDate.ValueChanged += (s, e) => ValueChanged?.Invoke(this, e);
            _dtpDate.Enter += (s, e) => this.Invalidate();
            _dtpDate.Leave += (s, e) => this.Invalidate();

            this.Controls.Add(_dtpDate);
        }
        protected override void AdjustLayout()
        {
            if (_dtpDate == null) return;

            int boxY = 22;
            int padding = 2;
            int nativeButtonWidth = 34; // العرض الدقيق لزر التقويم في الويندوز
            int iconSpace = 28;

            _dtpDate.RightToLeft = this.IsArabicLayout ? RightToLeft.Yes : RightToLeft.No;
            _dtpDate.RightToLeftLayout = this.IsArabicLayout;

            if (IsArabicLayout)
            {
                // 1. في العربي: الأداة تأخذ مساحتها الطبيعية تماماً دون أي زيادة
                // الأيقونة المخصصة على اليمين، وأداة التاريخ على اليسار
                _dtpDate.Location = new Point(padding, boxY + 2);
                _dtpDate.Width = this.Width - iconSpace - padding;

                // 2. البتر الجراحي: في النظام العربي، الزر الأصلي يقع في "أقصى اليسار" (من بكسل 0 إلى 34)
                // لذلك نجعل مقص الـ Region يبدأ من بعد الزر مباشرة (يبدأ من X = 34)
                int cropY = 2;
                int cropRightEdge = 3; // لقص الإطار الثلاثي الأبعاد من اليمين

                _dtpDate.Region = new Region(new Rectangle(
                    nativeButtonWidth, // تخطي الزر الأصلي تماماً (بتر)
                    cropY,
                    _dtpDate.Width - nativeButtonWidth - cropRightEdge,
                    _dtpDate.Height - (cropY * 2)
                ));
            }
            else
            {
                // 1. في الإنجليزي: الأداة تأخذ مساحتها الطبيعية
                // الأيقونة المخصصة على اليسار، وأداة التاريخ على اليمين
                _dtpDate.Location = new Point(iconSpace, boxY + 2);
                _dtpDate.Width = this.Width - iconSpace - padding;

                // 2. البتر الجراحي: في النظام الإنجليزي، الزر يقع في "أقصى اليمين"
                // نبدأ المقص بشكل طبيعي، ولكن نقلل العرض الكلي للمستطيل بمقدار الزر لكي نقصّه
                int cropX = 2;
                int cropY = 2;

                _dtpDate.Region = new Region(new Rectangle(
                    cropX,
                    cropY,
                    _dtpDate.Width - nativeButtonWidth - cropX, // طرح حجم الزر من العرض (بتر)
                    _dtpDate.Height - (cropY * 2)
                ));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int boxY = 22;
            int boxHeight = this.Height - boxY - 1;
            Rectangle boxRect = new Rectangle(0, boxY, this.Width - 1, boxHeight);

            bool isFocused = _dtpDate.Focused;
            Color currentBorderColor = isFocused ? _focusedBorderColor : _borderColor;

            using (SolidBrush bgBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(currentBorderColor, 1))
            {
                e.Graphics.FillRectangle(bgBrush, boxRect);
                e.Graphics.DrawRectangle(borderPen, boxRect);
            }

            string currentIconText = (isFocused && !string.IsNullOrEmpty(IconFocusedText)) ? IconFocusedText : IconText;

            if (!string.IsNullOrEmpty(currentIconText))
            {
                int iconWidth = 20;
                int iconSpace = 22; // يجب أن يتطابق مع الرقم في AdjustLayout

                // 3. التمركز الديناميكي: وضع الأيقونة بدقة في منتصف المساحة الفارغة
                int iconMargin = (iconSpace - iconWidth) / 2;
                int iconX = IsArabicLayout ? (this.Width - iconSpace + iconMargin) : iconMargin;

                RectangleF iconRect = new RectangleF(iconX, boxY, iconWidth, boxHeight);

                Color currentIconColor = isFocused ? IconFocusedColor : IconColor;

                using (StringFormat sfIcon = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (SolidBrush iconBrush = new SolidBrush(currentIconColor))
                {
                    e.Graphics.DrawString(currentIconText, _iconFont, iconBrush, iconRect, sfIcon);
                }
            }
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _dtpDate.Focus();
            SendKeys.Send("{F4}"); // اختصار الويندوز لفتح التقويم برمجياً عند النقر على الحقل
        }

        // =========================================================
        // تأمين دورة حياة الأداة (Lifecycle Safety)
        // =========================================================
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // إجبار الأداة على إعادة حساب إحداثياتها فور ظهورها الفعلي
            // هذا يمنع أي تداخل أو رسم خاطئ عند سحب الأداة لأول مرة
            AdjustLayout();
        }

        // =========================================================
        // تحرير الذاكرة (Garbage Collection)
        // =========================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iconFont?.Dispose(); // التخلص من الخط المخصص بشكل آمن
            }
            base.Dispose(disposing);
        }
    }
}