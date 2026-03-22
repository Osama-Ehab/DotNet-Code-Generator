using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using CodeGeneratorSolution.EmbeddedResources.UI.Interfaces;

namespace CodeGeneratorSolution.EmbeddedResources.UI.GenericForms
{
    public partial class frmGenericPopup : Form
    {
        // Constructor now takes the Interface, not raw UserControls
        public frmGenericPopup(List<IPopupBase> contentItems)
        {
            InitializeComponent();

            // 1. FORM SETUP
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. DYNAMIC TITLE
            // We pick the title from the first control, or join them if there are multiple
            this.Text = contentItems.Any()
                ? contentItems.First().WindowTitle
                : "Details";

            // 3. LAYOUT ENGINE
            FlowLayoutPanel mainLayout = new FlowLayoutPanel();
            mainLayout.FlowDirection = FlowDirection.TopDown;
            mainLayout.AutoSize = true;
            mainLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainLayout.Padding = new Padding(10);
            mainLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainLayout);

            // 4. ADD CONTENT
            // We calculate max width based on the Interface property
            int maxWidth = contentItems.Any()
                ? contentItems.Max(c => c.PreferredWidth)
                : 300;

            foreach (var item in contentItems)
            {
                var ctrl = item.AsUserControl(); // Get the actual UI component
                ctrl.Margin = new Padding(0, 0, 0, 10);

                // Safety: Ensure it doesn't shrink smaller than its preferred width
                ctrl.MinimumSize = new Size(item.PreferredWidth, 0);

                mainLayout.Controls.Add(ctrl);
            }

            // 5. ADD FOOTER (Close Button)
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(maxWidth, 40);

            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(80, 30);
            btnClose.Location = new Point(buttonPanel.Width - btnClose.Width, 0);
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnClose);
            mainLayout.Controls.Add(buttonPanel);
        }
        public frmGenericPopup(IPopupBase contentItem)
        {
            InitializeComponent();

            // 1. FORM SETUP
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. DYNAMIC TITLE
            // We pick the title from the first control, or join them if there are multiple
            this.Text = contentItem.WindowTitle ?? "Details";

            // 3. LAYOUT ENGINE
            FlowLayoutPanel mainLayout = new FlowLayoutPanel();
            mainLayout.FlowDirection = FlowDirection.TopDown;
            mainLayout.AutoSize = true;
            mainLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainLayout.Padding = new Padding(10);
            mainLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainLayout);

            // 4. ADD CONTENT
            // We calculate max width based on the Interface property
            int maxWidth = contentItem?.PreferredWidth ?? 300;


            var ctrl = contentItem.AsUserControl(); // Get the actual UI component
            ctrl.Margin = new Padding(0, 0, 0, 10);

            // Safety: Ensure it doesn't shrink smaller than its preferred width
            ctrl.MinimumSize = new Size(contentItem.PreferredWidth, 0);
            mainLayout.Controls.Add(ctrl);
            

            // 5. ADD FOOTER (Close Button)
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(maxWidth, 40);

            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(80, 30);
            btnClose.Location = new Point(buttonPanel.Width - btnClose.Width, 0);
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnClose);
            mainLayout.Controls.Add(buttonPanel);
        }
    }
}


