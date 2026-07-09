using ModernUI.Framework.Interfaces;
using ModernUI.Framework.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    public enum ModernIconPosition
    {
        Left,
        Right
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    public partial class ModernButton : Button, IHoverableIconControl
    {
        
        private bool _isHovered = false;
        private bool _isPressed = false;
        private string _iconText = "";
        private string _rawIconText = "";
        private string _iconHoverText = "";
        private string _rawIconHoverText = "";
        private Font _iconFont = new Font("Segoe MDL2 Assets", 12F, FontStyle.Regular);
        private ModernIconPosition _iconPosition = ModernIconPosition.Left;
        private bool _isArabicLayout = true;
        private Color _iconColor = Color.White;
        private Color _iconHoverColor = Color.White;

        [Category("Modern UI - Icons")]
        public string IconText
        {
            get => _rawIconText;
            set
            {
                _rawIconText = value;
                _iconText = ModernIconUtility.ParseIconText(value);
                this.Invalidate();
            }
        }

        [Category("Modern UI - Icons")]
        public string IconHoverText
        {
            get => _rawIconHoverText;
            set
            {
                _rawIconHoverText = value;
                _iconHoverText = ModernIconUtility.ParseIconText(value);
                this.Invalidate();
            }
        }

        [Category("Modern UI - Icons")]
        public Color IconColor
        {
            get => _iconColor;
            set
            {
                _iconColor = value;
                this.Invalidate();
            }
        }

        [Category("Modern UI - Icons")]
        public Color IconHoverColor
        {
            get => _iconHoverColor;
            set
            {
                _iconHoverColor = value;
                this.Invalidate();
            }
        }

        [Category("Modern UI - Icons")]
        public ModernIconPosition IconPosition
        {
            get => _iconPosition;
            set
            {
                _iconPosition = value;
                this.Invalidate();
            }
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

        public ModernButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(52, 152, 219);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
            this.Size = new Size(130, 40);
            
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; this.Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; this.Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { _isPressed = true; this.Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { _isPressed = false; this.Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color currentBgColor = this.BackColor;
            if (_isPressed)
                currentBgColor = ControlPaint.Dark(this.BackColor, 0.1f);
            else if (_isHovered)
                currentBgColor = ControlPaint.Light(this.BackColor, 0.2f);

            using (SolidBrush bgBrush = new SolidBrush(currentBgColor))
            {
                pevent.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            }

            string activeIconText = (_isHovered && !string.IsNullOrEmpty(_iconHoverText)) ? _iconHoverText : _iconText;
            Color activeIconColor = _isHovered ? _iconHoverColor : _iconColor;

            bool hasIcon = !string.IsNullOrEmpty(activeIconText);
            bool hasText = !string.IsNullOrEmpty(this.Text);
            int spacing = 8;

            SizeF iconSize = hasIcon ? pevent.Graphics.MeasureString(activeIconText, _iconFont) : SizeF.Empty;
            SizeF textSize = hasText ? pevent.Graphics.MeasureString(this.Text, this.Font) : SizeF.Empty;

            float totalWidth = (hasIcon ? iconSize.Width : 0) + (hasText ? textSize.Width : 0) + (hasIcon && hasText ? spacing : 0);
            float startX = (this.Width - totalWidth) / 2;

            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            using (SolidBrush iconBrush = new SolidBrush(activeIconColor))
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                float currentX = startX;

                bool drawIconFirst = (_iconPosition == ModernIconPosition.Left && !_isArabicLayout) ||
                                     (_iconPosition == ModernIconPosition.Right && _isArabicLayout);

                if (drawIconFirst)
                {
                    if (hasIcon)
                    {
                        RectangleF iconRect = new RectangleF(currentX, 0, iconSize.Width, this.Height);
                        pevent.Graphics.DrawString(activeIconText, _iconFont, iconBrush, iconRect, sf);
                        currentX += iconSize.Width + spacing;
                    }

                    if (hasText)
                    {
                        RectangleF textRect = new RectangleF(currentX, 0, textSize.Width, this.Height);
                        pevent.Graphics.DrawString(this.Text, this.Font, textBrush, textRect, sf);
                    }
                }
                else
                {
                    if (hasText)
                    {
                        RectangleF textRect = new RectangleF(currentX, 0, textSize.Width, this.Height);
                        pevent.Graphics.DrawString(this.Text, this.Font, textBrush, textRect, sf);
                        currentX += textSize.Width + spacing;
                    }

                    if (hasIcon)
                    {
                        RectangleF iconRect = new RectangleF(currentX, 0, iconSize.Width, this.Height);
                        pevent.Graphics.DrawString(activeIconText, _iconFont, iconBrush, iconRect, sf);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iconFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}