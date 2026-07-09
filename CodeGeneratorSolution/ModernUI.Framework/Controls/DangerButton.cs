using ModernUI.Framework.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    public partial class DangerButton : ModernButton
    {
        [DefaultValue("تجاهل")]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (value == this.Name)
                {
                    return;
                }
                base.Text = value;
            }
        }
        public DangerButton()
        {
            this.BackColor = Color.FromArgb(231, 76, 60);
            this.Text = "إلغاءوتجاهل"; 
            this.IconText = SegoeIcons.Cancel.Outline;
            this.IconColor = Color.White;
            this.IconHoverText = SegoeIcons.Cancel.Solid;
            this.IconHoverColor = Color.White;
            
        }
    }
}
