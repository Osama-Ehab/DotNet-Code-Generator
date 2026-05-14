using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGeneratorSolution.UI.CustomControls
{
    public partial class RecordLabel : Label
    {
        public RecordLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public void SetRecordsCount(int RecordsCount)
        {
           this.Text = (RecordsCount == 0) ? "# No Records Found" : $"# Total Records: {RecordsCount}";
        }
    }
}
