using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    // =========================================================================
    // 4. Data Grids (شبكات البيانات)
    // =========================================================================

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DataGridView))]
    public partial class ModernDataGridView : DataGridView
    {
        public ModernDataGridView()
        {
            // تفعيل Double Buffering لمنع الوميض باستخدام Reflection
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(this, true, null);

            this.EnableHeadersVisualStyles = false;
            this.BorderStyle = BorderStyle.None;
            this.BackgroundColor = Color.White;
            this.GridColor = Color.FromArgb(230, 230, 230);
            this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Header Style
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            this.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.ColumnHeadersHeight = 40;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Cell Style
            this.DefaultCellStyle.BackColor = Color.White;
            this.DefaultCellStyle.ForeColor = Color.Black;
            this.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            this.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            this.DefaultCellStyle.SelectionForeColor = Color.Black;
            this.RowTemplate.Height = 35;

            // Misc Config
            this.RowHeadersVisible = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
