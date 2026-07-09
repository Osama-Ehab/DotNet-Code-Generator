using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ModernUI.Framework.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    public partial class ModernTextBox : TextBox
    {
        // =========================================================
        // 1. P/Invoke Declarations (لإدارة النص الباهت)
        // =========================================================
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        // =========================================================
        // 2. Fields
        // =========================================================
        private string _placeholderText = "مثال";
        private bool _isArabicLayout = true;

        // =========================================================
        // 3. Properties
        // =========================================================
        [Category("Modern UI")]
        [Description("النص الإرشادي الباهت داخل المربع")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                UpdateCueBanner();
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
            }
        }

        // =========================================================
        // 4. Constructor
        // =========================================================
        public ModernTextBox()
        {
            this.Font = new Font("Segoe UI", 10F);
            this.BorderStyle = BorderStyle.None;
            this.Size = new Size(187, 30);
        }

        // =========================================================
        // 5. Core Methods (إدارة النص الباهت)
        // =========================================================
        private void UpdateCueBanner()
        {
            if (this.IsHandleCreated && !string.IsNullOrEmpty(_placeholderText))
            {
                SendMessage(this.Handle, EM_SETCUEBANNER, 1, _placeholderText);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCueBanner();
        }

        //// =========================================================
        //// 6. UX Visual Feedback (تأثيرات التركيز)
        //// =========================================================
        //protected override void OnEnter(EventArgs e)
        //{
        //    base.OnEnter(e);
        //    this.BackColor = Color.White;
        //}

        //protected override void OnLeave(EventArgs e)
        //{
        //    base.OnLeave(e);
        //    this.BackColor = Color.WhiteSmoke;
        //}
    }
}