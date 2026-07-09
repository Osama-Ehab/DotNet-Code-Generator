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
    public partial class SuccessButton : ModernButton
    {
        [DefaultValue("تأكيد وحفظ")]
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
        public SuccessButton()
        {
            this.BackColor = Color.FromArgb(46, 204, 113);
            this.Text = "تأكيد وحفظ";
            this.IconText = SegoeIcons.Save.Outline;
            this.IconHoverText = SegoeIcons.Save.Solid;
            this.IconColor = Color.White;
            this.IconHoverColor = Color.FromArgb(39, 174, 96);
        }
    }
}
