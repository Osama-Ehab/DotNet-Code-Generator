
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ModernUI.Framework.Enums;
using ModernUI.Framework.Icons;
using ModernUI.Framework.Interfaces;
using ModernUI.Framework.Util;

namespace ModernUI.Framework.Controls
    {
        [ToolboxItem(true)]
        [DefaultProperty("TextValue")]
        public partial class ModernInputGroup : ModernControlBase, IFocusableIconControl
        {
            private ModernTextBox _txtInput;
            private InputValidationMode _validationMode = InputValidationMode.General;

            private string _rawIconText = SegoeIcons.Person.Outline;
            private string _iconText = "";

            // تغيير المسميات لتتوافق مع حالة التركيز (Focus)
            private string _rawIconFocusedText = SegoeIcons.Person.Solid;
            private string _iconFocusedText = "";

            private Font _iconFont = new Font("Segoe MDL2 Assets", 11F, FontStyle.Regular);
            private Color _iconColor = Color.FromArgb(150, 150, 150);
            private Color _iconFocusedColor = Color.FromArgb(52, 152, 219);

            // --- Interface Implementations ---
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

            [Category("Data Validation")]
            public InputValidationMode ValidationMode
            {
                get => _validationMode;
                set => _validationMode = value;
            }

            [Category("Modern UI")]
            public string PlaceholderText
            {
                get => _txtInput.PlaceholderText;
                set => _txtInput.PlaceholderText = value;
            }

            [Category("Modern UI")]
            public string TextValue
            {
                get => _txtInput.Text;
                set => _txtInput.Text = value;
            }

            public ModernInputGroup()
            {
                this.Size = new Size(203, 52);

                _txtInput = new ModernTextBox();
                _txtInput.BorderStyle = BorderStyle.None;
                _txtInput.BackColor = Color.White;
                _txtInput.Height = 22;

                // الاعتماد كلياً على أحداث التركيز لتحديث الرسم
                _txtInput.Enter += (s, e) => { OnTextBoxEnter(s, e); this.Invalidate(); };
                _txtInput.Leave += (s, e) => this.Invalidate();
                _txtInput.KeyPress += OnTextBoxKeyPress;

                this.Controls.Add(_txtInput);

                // تهيئة النصوص بعد معالجتها
                IconText = _rawIconText;
                IconFocusedText = _rawIconFocusedText;
            }
          

            protected override void AdjustLayout()
            {
                if (_txtInput == null) return;

                int padding = 5;
                // التحقق من وجود أيقونة عادية أو أيقونة تركيز لحجز المساحة
                int iconSpace = (string.IsNullOrEmpty(_iconText) && string.IsNullOrEmpty(_iconFocusedText)) ? 0 : 28;
                int textBoxY = this.Height - _txtInput.Height - 5;

                if (IsArabicLayout)
                {
                    _txtInput.Width = this.Width - iconSpace - (padding * 2);
                    _txtInput.Location = new Point(padding, textBoxY);
                }
                else
                {
                    _txtInput.Width = this.Width - iconSpace - (padding * 2);
                    _txtInput.Location = new Point(padding + iconSpace, textBoxY);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                // استدعاء الكلاس الأساسي لرسم العنوان (Label)
                base.OnPaint(e);

                int boxY = 22;
                int boxHeight = this.Height - boxY - 1;
                Rectangle boxRect = new Rectangle(0, boxY, this.Width - 1, boxHeight);

                // تحديد حالة التركيز
                bool isFocused = _txtInput.Focused;
                Color currentBorderColor = isFocused ? _focusedBorderColor : _borderColor;

                // رسم الخلفية والحدود
                using (SolidBrush bgBrush = new SolidBrush(Color.White))
                using (Pen borderPen = new Pen(currentBorderColor, 1))
                {
                    e.Graphics.FillRectangle(bgBrush, boxRect);
                    e.Graphics.DrawRectangle(borderPen, boxRect);
                }

                // رسم الأيقونة بناءً على حالة التركيز فقط
                string currentIconText = (isFocused && !string.IsNullOrEmpty(_iconFocusedText)) ? _iconFocusedText : _iconText;

                if (!string.IsNullOrEmpty(currentIconText))
                {
                    int iconWidth = 20;
                    int iconX = IsArabicLayout ? (this.Width - iconWidth - 2) : 2;
                    RectangleF iconRect = new RectangleF(iconX, boxY, iconWidth, boxHeight);

                    Color currentIconColor = isFocused ? _iconFocusedColor : _iconColor;

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
                _txtInput.Focus(); // تمرير التركيز للـ TextBox الداخلي عند النقر على أي مكان في الأداة
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _iconFont?.Dispose();
                }
                base.Dispose(disposing);
            }

            // =========================================================
            // دوال الإدخال الذكية (Validation & Culture)
            // =========================================================
            private void OnTextBoxEnter(object sender, EventArgs e)
            {
                string culture = (_validationMode == InputValidationMode.ArabicOnly)?  "ar-EG" : "en-US";

                if (culture != null)
                {
                    try
                    {
                        InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo(culture));
                    }
                    catch { /* تجاهل الخطأ بصمت */ }
                }
            }

            private void OnTextBoxKeyPress(object sender, KeyPressEventArgs e)
            {
                bool isArabic = e.KeyChar >= 0x0600 && e.KeyChar <= 0x06FF;
                bool isNumber = char.IsDigit(e.KeyChar);
                bool isControl = char.IsControl(e.KeyChar);

                switch (_validationMode)
                {
                    case InputValidationMode.ArabicOnly:
                        if (!isArabic && !isControl) e.Handled = true;
                        break;

                    case InputValidationMode.EnglishOnly:
                        if (isArabic) e.Handled = true;
                        break;

                    case InputValidationMode.NumericOnly:
                        if (!isNumber && !isControl) e.Handled = true;
                        break;
                }
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