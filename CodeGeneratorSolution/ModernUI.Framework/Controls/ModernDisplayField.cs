using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ModernUI.Framework.Interfaces;

namespace ModernUI.Framework.Controls
{
    public partial class ModernDisplayField : ModernControlBase, IIconableControl
    {
        private TextBox _txtValue;
        private string _iconText = "💡";
        private Color _iconColor = Color.FromArgb(100, 100, 100);

        public string IconText
        {
            get => _iconText;
            set { _iconText = value; this.Invalidate(); }
        }

        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; this.Invalidate(); }
        }

        // الخاصية التي سيتعامل معها المبرمج في الكود المُولد
        public string Value
        {
            get => _txtValue.Text;
            set => _txtValue.Text = value;
        }

        // كشف الـ TextBox لمنح المطور حرية تلوين النص أو تغيير الخط وقت التشغيل
        public TextBox InnerTextBox => _txtValue;

        public ModernDisplayField()
        {
            // إنشاء المقبض الوحيد (The only Window Handle)
            _txtValue = new TextBox
            {
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.IBeam,
                Multiline = true
            };

            this.Controls.Add(_txtValue);
            this.Height = 55; // الارتفاع الافتراضي (Label + TextBox + Separator)
            this.Margin = new Padding(5, 5, 5, 15); // مسافات خارجية بين الحقول
        }

        protected override void AdjustLayout()
        {
            // الـ Label الأساسي الخاص بك يأخذ أعلى 22 بكسل
            int topPadding = 24;
            int iconWidth = 35;

            if (IsArabicLayout)
            {
                // الأيقونة على اليمين، والـ TextBox يأخذ باقي المساحة يساراً
                _txtValue.Location = new Point(5, topPadding);
                _txtValue.Width = this.Width - iconWidth - 10;
            }
            else
            {
                // الأيقونة على اليسار، والـ TextBox يأخذ المساحة يميناً
                _txtValue.Location = new Point(iconWidth, topPadding);
                _txtValue.Width = this.Width - iconWidth - 10;
            }

            // ترك 2 بكسل بالأسفل من أجل الخط الفاصل
            _txtValue.Height = this.Height - topPadding - 2;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 1. رسم العنوان (LabelText) عبر الكلاس الأساسي الخاص بك
            base.OnPaint(e);

            // 2. رسم الأيقونة (GDI+) بجوار الـ TextBox مباشرة
            using (Font iconFont = new Font("Segoe UI", 12F)) // أو خط ModernUI Icons الخاص بك
            using (SolidBrush iconBrush = new SolidBrush(_iconColor))
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
            {
                RectangleF iconRect;
                if (IsArabicLayout)
                {
                    // رسم الأيقونة في أقصى اليمين تحت العنوان
                    iconRect = new RectangleF(this.Width - 35, 24, 35, this.Height - 24);
                }
                else
                {
                    // رسم الأيقونة في أقصى اليسار
                    iconRect = new RectangleF(0, 24, 35, this.Height - 24);
                }

                e.Graphics.DrawString(_iconText, iconFont, iconBrush, iconRect, sf);
            }

            // 3. رسم الخط الفاصل السفلي (Separator) باحترافية
            using (Pen linePen = new Pen(Color.FromArgb(235, 235, 235), 1))
            {
                e.Graphics.DrawLine(linePen, 5, this.Height - 1, this.Width - 5, this.Height - 1);
            }
        }
    }
}