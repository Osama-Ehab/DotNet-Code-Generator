using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    // =========================================================================
    // 3. Toggles & Switches (أدوات التبديل)
    // =========================================================================

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(CheckBox))] // أيقونة CheckBox قياسية
    [DefaultEvent("CheckedChanged")]
    public partial class ModernToggle : CheckBox
    {
        private Color onBackColor = Color.FromArgb(46, 204, 113);
        private Color onToggleColor = Color.White;
        private Color offBackColor = Color.FromArgb(189, 195, 199);
        private Color offToggleColor = Color.White;

        public ModernToggle()
        {
            this.MinimumSize = new Size(45, 22);
            this.Size = new Size(60, 25);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.Text = ""; // مسح النص الافتراضي
        }

        private GraphicsPath GetFigurePath()
        {
            int arcSize = this.Height - 1;
            Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
            Rectangle rightArc = new Rectangle(this.Width - arcSize - 2, 0, arcSize, arcSize);

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            int toggleSize = this.Height - 5;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(this.Parent != null ? this.Parent.BackColor : Color.White);

            // [حماية معمارية]: GraphicsPath يستهلك موارد Unmanaged، يجب تدميره بعد استخدامه
            using (GraphicsPath path = GetFigurePath())
            {
                if (this.Checked)
                {
                    // [حماية معمارية]: تدمير الفرش (Brushes) لمنع تسريب الذاكرة (Memory Leak)
                    using (SolidBrush bgBrush = new SolidBrush(onBackColor))
                    using (SolidBrush toggleBrush = new SolidBrush(onToggleColor))
                    {
                        pevent.Graphics.FillPath(bgBrush, path);
                        pevent.Graphics.FillEllipse(toggleBrush,
                            new Rectangle(this.Width - this.Height + 1, 2, toggleSize, toggleSize));
                    }
                }
                else
                {
                    using (SolidBrush bgBrush = new SolidBrush(offBackColor))
                    using (SolidBrush toggleBrush = new SolidBrush(offToggleColor))
                    {
                        pevent.Graphics.FillPath(bgBrush, path);
                        pevent.Graphics.FillEllipse(toggleBrush,
                            new Rectangle(2, 2, toggleSize, toggleSize));
                    }
                }
            }
        }

    }
}
