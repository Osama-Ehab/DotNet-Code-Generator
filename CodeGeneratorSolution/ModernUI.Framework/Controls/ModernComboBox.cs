using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ModernUI.Framework.Icons;
using ModernUI.Framework.Interfaces;
using ModernUI.Framework.Util;

namespace ModernUI.Framework.Controls
{
    [DefaultEvent("SelectedIndexChanged")]
    [ToolboxItem(true)]
    public partial class ModernComboBox : ModernControlBase, IFocusableIconControl
    {
        // 1. الأداة الداخلية الحقيقية (Native Control)
        private ComboBox _cmbData;

        // =========================================================
        // 2. نظام الأيقونات وتطبيق الواجهة (IFocusableIconControl)
        // =========================================================
        private string _rawIconText = SegoeIcons.Search.Outline; // أيقونة افتراضية للبحث/القوائم
        private string _iconText = "";

        private string _rawIconFocusedText = SegoeIcons.Search.Solid;
        private string _iconFocusedText = "";

        private Font _iconFont = new Font("Segoe MDL2 Assets", 11F, FontStyle.Regular);
        private Color _iconColor = Color.FromArgb(150, 150, 150);
        private Color _iconFocusedColor = Color.FromArgb(52, 152, 219);

        public event EventHandler SelectedIndexChanged;

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

      

        // =========================================================
        // 3. تغليف خصائص الـ ComboBox (Bridged Properties)
        // =========================================================
        // ملاحظة: تم حل مشكلة الـ Stack Overflow بتوجيه الخصائص إلى _cmbData بدلاً من this

        [Category("Modern UI - Data")]
        public object DataSource
        {
            get => _cmbData.DataSource;
            set => _cmbData.DataSource = value;
        }

        [Category("Modern UI - Data")]
        public string DisplayMember
        {
            get => _cmbData.DisplayMember;
            set => _cmbData.DisplayMember = value;
        }

        [Category("Modern UI - Data")]
        public string ValueMember
        {
            get => _cmbData.ValueMember;
            set => _cmbData.ValueMember = value;
        }

        [Category("Modern UI - Data")]
        [Browsable(false)]
        public object SelectedValue
        {
            get => _cmbData.SelectedValue;
            set => _cmbData.SelectedValue = value;
        }

        [Category("Modern UI - Data")]
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _cmbData.SelectedIndex;
            set => _cmbData.SelectedIndex = value;
        }

        [Category("Modern UI")]
        public string TextValue
        {
            get => _cmbData.Text;
            set => _cmbData.Text = value;
        }

        [Category("Modern UI - Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBox.ObjectCollection Items => _cmbData.Items;

        // الوصول المباشر للأداة الداخلية إذا دعت الحاجة لبرمجة متقدمة
        [Browsable(false)]
        public ComboBox InnerComboBox => _cmbData;

        // =========================================================
        // 4. البناء والتهيئة (Constructor)
        // =========================================================
        public ModernComboBox()
        {
            this.Size = new Size(203, 52);

            _cmbData = new ComboBox();
            _cmbData.FlatStyle = FlatStyle.Flat;
            _cmbData.BackColor = Color.White;
            _cmbData.Font = new Font("Segoe UI", 10.5F);

            // إعدادات البحث الذكي
            _cmbData.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _cmbData.AutoCompleteSource = AutoCompleteSource.ListItems;
            _cmbData.DropDownStyle = ComboBoxStyle.DropDown;

            // ربط الأحداث بالتركيز ليتم إعادة رسم الحدود والأيقونة
            _cmbData.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, e);
            _cmbData.Enter += (s, e) => this.Invalidate();
            _cmbData.Leave += (s, e) => this.Invalidate();
            _cmbData.Validating += OnComboBoxValidating;

            this.Controls.Add(_cmbData);

            IconText = _rawIconText;
            IconFocusedText = _rawIconFocusedText;
        }

        // =========================================================
        // 5. هندسة المساحات (Layout Logic)
        // =========================================================
        protected override void AdjustLayout()
        {
            if (_cmbData == null) return;

            int boxY = 22; // تأمين موقع الأداة أسفل العنوان
            int padding = 2;
            int iconSpace = 24; // المساحة المحجوزة لأيقونتنا

            // إجبار الأداة الداخلية على احترام لغة الواجهة
            _cmbData.RightToLeft = this.IsArabicLayout ? RightToLeft.Yes : RightToLeft.No;

            if (IsArabicLayout)
            {
                // في العربي: سهم الويندوز يذهب لليسار تلقائياً.
                // نترك مساحة (iconSpace) على اليمين لرسم أيقونتنا.
                _cmbData.Location = new Point(padding, boxY + 2);
                _cmbData.Width = this.Width - iconSpace - (padding * 2);
            }
            else
            {
                // في الإنجليزي: سهم الويندوز يذهب لليمين تلقائياً.
                // نزيح الأداة لليمين لنترك مساحة (iconSpace) على اليسار لأيقونتنا.
                _cmbData.Location = new Point(iconSpace, boxY + 2);
                _cmbData.Width = this.Width - iconSpace - (padding * 2);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int boxY = 22;
            int boxHeight = this.Height - boxY - 1;
            Rectangle boxRect = new Rectangle(0, boxY, this.Width - 1, boxHeight);

            bool isFocused = _cmbData.Focused;
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
                // ضبط مكان الأيقونة بشكل ثابت لا يقبل الخطأ
                int iconWidth = 20;
                int iconX = IsArabicLayout ? (this.Width - iconWidth - 4) : 4;
                RectangleF iconRect = new RectangleF(iconX, boxY, iconWidth, boxHeight);

                Color currentIconColor = isFocused ? IconFocusedColor : IconColor;

                using (StringFormat sfIcon = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (SolidBrush iconBrush = new SolidBrush(currentIconColor))
                {
                    e.Graphics.DrawString(currentIconText, _iconFont, iconBrush, iconRect, sfIcon);
                }
            }
        }
        // =========================================================
        // 7. حماية البيانات ودورة الحياة
        // =========================================================
        private void OnComboBoxValidating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_cmbData.Text)) return;

            int index = _cmbData.FindStringExact(_cmbData.Text);
            if (index == -1)
            {
                // منع المستخدم من إدخال قيمة غير موجودة في القائمة
                _cmbData.Text = string.Empty;
                _cmbData.SelectedIndex = -1;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _cmbData.Focus();
            // فتح القائمة المنسدلة فور النقر على أي مكان في الأداة لتسريع تجربة المستخدم
            _cmbData.DroppedDown = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iconFont?.Dispose(); // التخلص من الخط المخصص بشكل آمن
            }
            base.Dispose(disposing);
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
    }
}