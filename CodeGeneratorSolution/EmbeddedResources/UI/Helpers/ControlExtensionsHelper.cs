using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Helpers
{
    public static class ControlExtensionsHelper
    {
        // ==========================================
        // 1. VALIDATION & ERROR UX
        // ==========================================
        public static void SetErrorAndFocus(this Control control, ErrorProvider errorProvider, string message)
        {
            errorProvider?.SetError(control, message);
            control?.Focus();

            // UX Upgrade: If it's a TextBox, highlight the bad text so they can instantly type over it!
            if (control is TextBox txt)
            {
                txt.SelectAll();
            }
        }

        public static void ClearError(this Control control, ErrorProvider errorProvider)
        {
            errorProvider?.SetError(control, string.Empty);
        }

        // Architectural Upgrade: Recursively clears errors for an entire Form, Panel, or UserControl!
        public static void ClearAllErrors(this Control container, ErrorProvider errorProvider)
        {
            foreach (Control c in container.Controls)
            {
                errorProvider?.SetError(c, string.Empty);

                // If this control has children (like a GroupBox or Panel), dive into it
                if (c.HasChildren)
                {
                    c.ClearAllErrors(errorProvider);
                }
            }
        }

        // ==========================================
        // 2. EVENT ROUTING
        // ==========================================
        public static void HandleEnterKey(this Control control, Action action)
        {
            // We use a named local function instead of a lambda.
            // This allows us to cleanly detach it if needed, or simply keep the logic clean.
            control.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Stops the annoying Windows "Ding!" sound
                    action?.Invoke();
                }
            };
        }

        // ==========================================
        // 3. ASYNC/AWAIT THREAD SAFETY
        // ==========================================
        // Prevents Cross-Thread operation crashes when updating the UI after a database call
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }

        // ==========================================
        // 4. INPUT RESTRICTION (Data Integrity)
        // ==========================================

        /// <summary>
        /// Forces a TextBox to only accept numeric digits and control characters (like Backspace).
        /// Perfect for National ID, Phone Numbers, or Integer Foreign Keys.
        /// </summary>
        public static void OnlyNumbers(this TextBox textBox)
        {
            textBox.KeyPress += (sender, e) =>
            {
                // Allow control characters (Backspace) and digits
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // "Swallow" the keypress so it doesn't appear in the box
                }
            };
        }

        /// <summary>
        /// Forces a TextBox to accept valid decimals (Numbers, Backspace, and exactly ONE decimal point).
        /// Perfect for Application Fees, Fines, and Payments.
        /// </summary>
        public static void OnlyDecimals(this TextBox textBox)
        {
            textBox.KeyPress += (sender, e) =>
            {
                // Allow control chars and digits
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // Only allow ONE decimal point
                if (e.KeyChar == '.' && textBox.Text.Contains('.'))
                {
                    e.Handled = true;
                }
            };
        }

        // ==========================================
        // 5. UX ENHANCEMENTS (Flow and Polish)
        // ==========================================

        /// <summary>
        /// Maps the Enter key to the Tab key behavior. 
        /// Massive UX upgrade for data-entry heavy forms (like adding a new Person).
        /// </summary>
        public static void EnterToTab(this Control control)
        {
            control.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Stop the "ding" sound
                    SendKeys.Send("{TAB}");    // Magically move to the next control
                }
            };
        }

        // --- P/Invoke for Native Windows Watermarks ---
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Adds modern gray placeholder text (a Watermark) to a standard WinForms TextBox.
        /// </summary>
        public static void SetPlaceholder(this TextBox textBox, string placeholderText)
        {
            // Send a native Windows message to the TextBox handle to set the cue banner
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholderText);
        }
    }
}
