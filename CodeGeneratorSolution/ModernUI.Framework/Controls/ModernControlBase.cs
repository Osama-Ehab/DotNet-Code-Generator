using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using ModernUI.Framework.Interfaces;

namespace ModernUI.Framework.Controls
{
    public abstract partial class ModernControlBase : Control, IModernControl
    {
        private string _labelText = "عنوان الحقل";
        private bool _isArabicLayout = true;

        // Protected fields so derived classes can use them
        protected Font _labelFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        protected Color _labelColor = Color.FromArgb(64, 64, 64);
        protected Color _borderColor = Color.FromArgb(200, 200, 200);
        protected Color _focusedBorderColor = Color.FromArgb(52, 152, 219);

        [Category("Modern UI")]
        public string LabelText
        {
            get => _labelText;
            set { _labelText = value; this.Invalidate(); }
        }

        [Category("Modern UI")]
        public bool IsArabicLayout
        {
            get => _isArabicLayout;
            set
            {
                _isArabicLayout = value;
                this.RightToLeft = value ? RightToLeft.Yes : RightToLeft.No;
                AdjustLayout();
                this.Invalidate();
            }
        }

        public ModernControlBase()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }

        // Abstract method forcing child controls to define their layout math
        protected abstract void AdjustLayout();

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
        }

        // Centralized GDI+ Painting for the common elements
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Draw the Label globally for all derived controls
            RectangleF textRect = new RectangleF(2, 0, this.Width - 4,22);
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            using (SolidBrush textBrush = new SolidBrush(_labelColor))
            {
                sf.Alignment = StringAlignment.Near;
                if (_isArabicLayout) sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                e.Graphics.DrawString(_labelText, _labelFont, textBrush, textRect, sf);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _labelFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}