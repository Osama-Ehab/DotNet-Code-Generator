using ModernUI.Framework.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RadioButton))]
    public partial class ModernRadioButton : RadioButton, IModernControl
    {
        private Color _checkedColor = Color.FromArgb(52, 152, 219);
        private Color _unCheckedColor = Color.FromArgb(150, 150, 150);
        private bool _isArabicLayout = true;

        [Category("Modern UI")]
        public Color CheckedColor
        {
            get => _checkedColor;
            set { _checkedColor = value; this.Invalidate(); }
        }

        [Category("Modern UI")]
        public Color UnCheckedColor
        {
            get => _unCheckedColor;
            set { _unCheckedColor = value; this.Invalidate(); }
        }

        [Category("Modern UI")]
        public bool IsArabicLayout
        {
            get => _isArabicLayout;
            set
            {
                _isArabicLayout = value;
                this.RightToLeft = value ? RightToLeft.Yes : RightToLeft.No;
                this.Invalidate();
            }
        }

        public ModernRadioButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 10F);
            this.AutoSize = false;
            this.MinimumSize = new Size(0, 25);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            pevent.Graphics.Clear(this.Parent != null ? this.Parent.BackColor : Color.White);

            int circleSize = 16;
            int circleY = (this.Height - circleSize) / 2;
            int circleX = _isArabicLayout ? (this.Width - circleSize - 1) : 1;

            Rectangle circleRect = new Rectangle(circleX, circleY, circleSize, circleSize);

            Color currentColor = this.Checked ? _checkedColor : _unCheckedColor;

            using (Pen borderPen = new Pen(currentColor, 2f))
            {
                pevent.Graphics.DrawEllipse(borderPen, circleRect);
            }

            if (this.Checked)
            {
                int innerSize = 8;
                int innerOffset = (circleSize - innerSize) / 2;
                Rectangle innerRect = new Rectangle(circleX + innerOffset, circleY + innerOffset, innerSize, innerSize);

                using (SolidBrush innerBrush = new SolidBrush(currentColor))
                {
                    pevent.Graphics.FillEllipse(innerBrush, innerRect);
                }
            }

            int textX = _isArabicLayout ? 0 : circleSize + 8;
            int textWidth = this.Width - circleSize - 8;
            RectangleF textRect = new RectangleF(textX, 0, textWidth, this.Height);

            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                sf.Alignment = _isArabicLayout ? StringAlignment.Far : StringAlignment.Near;
                pevent.Graphics.DrawString(this.Text, this.Font, textBrush, textRect, sf);
            }
        }
    }
}