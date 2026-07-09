using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using ModernUI.Framework.Icons;
using ModernUI.Framework.Util;

namespace ModernUI.Framework.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("Title")]
    public partial class ModernControlHeader : Control
    {
        // =========================================================
        // متغيرات محرك الرسم (GDI+ Fields) - لا يوجد أدوات داخلية!
        // =========================================================
        private string _title = "عنوان الشاشة";
        private Font _titleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        private Color _titleColor = Color.FromArgb(45, 45, 48);

        // دعم الأيقونات النصية (للحفاظ على دقة عالية وأداء صاروخي)
        // =========================================================
        // متغيرات الأيقونة (تطبيق نمط Raw vs Parsed)
        // =========================================================
        private string _rawIconText = SegoeIcons.Default.Solid;
        private string _iconText = "";
        private Font _iconFont = new Font("Segoe MDL2 Assets", 28F, FontStyle.Regular);
        private Color _iconColor = Color.FromArgb(52, 152, 219);

        // دعم الصور التقليدية (مثل شعار المدرسة)
        private Image _headerImage = null;

        // =========================================================
        // الخصائص (Properties)
        // =========================================================
        [Category("Modern UI")]
        public string Title
        {
            get => _title;
            set { _title = value; this.Invalidate(); } // Invalidate تطلب من الويندوز إعادة الرسم
        }

        [Category("Modern UI")]
        public Color TitleColor
        {
            get => _titleColor;
            set { _titleColor = value; this.Invalidate(); }
        }

        [Category("Modern UI")]
        public Font TitleFont
        {
            get => _titleFont;
            set { _titleFont = value; this.Invalidate(); }
        }

   
 

        // =========================================================
        // الخاصية المحدثة
        // =========================================================
        [Category("Modern UI - Icons")]
        [Description("كود الأيقونة من Segoe MDL2 (اتركه فارغاً إذا كنت تستخدم Image)")]
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
        public Color IconColor
        {
            get => _iconColor;
            set { _iconColor = value; this.Invalidate(); }
        }

        [Category("Modern UI - Image")]
        [Description("صورة الترويسة (تلغي عمل الأيقونة النصية إذا تم وضعها)")]
        public Image HeaderImage
        {
            get => _headerImage;
            set { _headerImage = value; this.Invalidate(); }
        }

        // =========================================================
        // التهيئة (Constructor)
        // =========================================================
        public ModernControlHeader()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(400, 100);
            this.BackColor = Color.Transparent; // لكي يندمج مع خلفية الشاشة
            IconText = _rawIconText; // تهيئة الأيقونة وتحويلها مبكراً
        }

        // =========================================================
        // محرك الرسم الذكي (GDI+ Painting Engine)
        // =========================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // تحسين جودة الحواف (Anti-Aliasing) لدعم دقات الشاشة العالية في .NET 10
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int currentY = 5; // نقطة البداية من الأعلى

            // 1. رسم الصورة أو الأيقونة
            if (_headerImage != null)
            {
                // إذا كان هناك صورة، نرسمها في المنتصف
                int imgX = (this.Width - _headerImage.Width) / 2;
                e.Graphics.DrawImage(_headerImage, new Point(imgX, currentY));
                currentY += _headerImage.Height + 5;
            }
            else if (!string.IsNullOrEmpty(_iconText))
            {
                // إذا لم يكن هناك صورة، نرسم الأيقونة النصية (أسرع وأخف)
                SizeF iconSize = e.Graphics.MeasureString(_iconText, _iconFont);
                RectangleF iconRect = new RectangleF(0, currentY, this.Width, iconSize.Height);

                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                using (SolidBrush iconBrush = new SolidBrush(_iconColor))
                {
                    e.Graphics.DrawString(_iconText, _iconFont, iconBrush, iconRect, sf);
                }
                currentY += (int)iconSize.Height;
            }

            // 2. رسم العنوان (Title)
            if (!string.IsNullOrEmpty(_title))
            {
                RectangleF textRect = new RectangleF(0, currentY, this.Width, this.Height - currentY);
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                using (SolidBrush textBrush = new SolidBrush(_titleColor))
                {
                    e.Graphics.DrawString(_title, _titleFont, textBrush, textRect, sf);
                }
            }
        }

        // تحرير موارد نظام الرسم عند إغلاق الشاشة
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose();
                _iconFont?.Dispose();
                _headerImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}