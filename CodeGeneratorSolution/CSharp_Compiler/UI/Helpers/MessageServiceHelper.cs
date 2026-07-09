using CodeGeneratorSolution.Application;
using CodeGeneratorSolution.Core.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGeneratorSolution.UI.Helpers
{
    public static class MessageServiceHelper
    {
        #region Standard MessageBoxes


        public static void PopupMessage(Result result)
        {
            // 1. Guard Clause for Success
            // If it's a success, we show a non-blocking Toast notification
            // so we don't annoy the user with a popup they have to click "OK" on.
            if (result.IsSuccess)
            {
                ShowToastSuccess("Operation completed successfully.");
                return;
            }

            // 2. The Error Router
            // Maps the specific architectural error to the correct UI presentation
            switch (result.ErrorType)
            {
                case ErrorType.Validation:
                    // Missing fields, bad formatting
                    ShowValidationError(result.ErrorMessage);
                    break;

                case ErrorType.NotFound:
                    // Searched for a Driver ID that doesn't exist
                    ShowWarning(result.ErrorMessage, "Record Not Found");
                    break;

                case ErrorType.Conflict:
                    // Tried to add a NationalNo that already exists
                    ShowWarning(result.ErrorMessage, "Data Conflict Detected");
                    break;

                case ErrorType.ConstraintViolation:
                    // Tried to delete a User who has active licenses attached
                    ShowWarning(result.ErrorMessage, "Action Denied");
                    break;

                case ErrorType.Unauthorized:
                    // Tried to open a screen without permissions
                    ShowError(result.ErrorMessage, "Unauthorized Access");
                    break;

                case ErrorType.Database:
                    // SQL Server went offline or timed out
                    ShowError(result.ErrorMessage, "Database Connection Error");
                    break;

                case ErrorType.Unexpected:
                default:
                    // Null reference exceptions, massive system crashes
                    ShowError(result.ErrorMessage, "System Error");
                    break;
            }
        }


        public static void ShowError(string message, string title = "Error") =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool ShowErrorReturnBoolean(string message, string title = "Error",bool returnValue = false)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return returnValue;
        }

        public static void ShowValidationError(string message) =>
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void ShowInfo(string message, string title = "Info") =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowSuccess(string message, string title = "Success") =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowWarning(string message, string title = "Warning") =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static bool Confirm(string message, string title = "Confirm") =>
            MessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;

        #endregion

        #region Toast Notifications (Modern, Lightweight & Stackable)


        // Thematic UI Wrappers
        public static void ShowToastInfo(string message, string title = "Information", int duration = 3000) =>
            ShowToast(message, title, duration, Color.FromArgb(41, 128, 185)); // Flat Peter River Blue

        public static void ShowToastSuccess(string message, string title = "Success", int duration = 3000) =>
            ShowToast(message, title, duration, Color.FromArgb(39, 174, 96)); // Flat Nephritis Green

        public static void ShowToastWarning(string message, string title = "Warning", int duration = 3000) =>
            ShowToast(message, title, duration, Color.FromArgb(230, 126, 34)); // Flat Carrot Orange

        public static void ShowToastError(string message, string title = "Error", int duration = 4000) => // Errors stay slightly longer
            ShowToast(message, title, duration, Color.FromArgb(192, 57, 43)); // Flat Pomegranate Red


        // 1. القائمة التي ستراقب كل الإشعارات النشطة
        private static readonly System.Collections.Generic.List<Form> _activeToasts = new System.Collections.Generic.List<Form>();
        private static bool IsArabicLayout = true;

        public static async void ShowToast(string message, string title = "", int duration = 3000, Color? backColor = null, Color? foreColor = null)
        {
            var toast = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = backColor ?? Color.FromArgb(45, 45, 48),
                Size = new Size(320, 85),
                Opacity = 0, // يبدأ مخفياً للأنيميشن
                RightToLeft = IsArabicLayout ? RightToLeft.Yes : RightToLeft.No,
                Padding = new Padding(10)
            };

            Font titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font messageFont = new Font("Segoe UI", 9);

            var lblTitle = new Label
            {
                Text = title,
                Font = titleFont,
                ForeColor = foreColor ?? Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = IsArabicLayout ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft
            };

            var lblMessage = new Label
            {
                Text = message,
                Font = messageFont,
                ForeColor = foreColor ?? Color.FromArgb(220, 220, 220),
                Dock = DockStyle.Fill,
                TextAlign = IsArabicLayout ? ContentAlignment.TopRight : ContentAlignment.TopLeft,
                Padding = new Padding(0, 5, 0, 0)
            };

            toast.Controls.Add(lblMessage);
            toast.Controls.Add(lblTitle);

            // 2. إضافة الإشعار الجديد للقائمة وإعادة ترتيب الشاشة
            _activeToasts.Add(toast);
            RepositionToasts();

            // 3. التنظيف الصارم عند الإغلاق
            toast.FormClosed += (s, e) =>
            {
                _activeToasts.Remove(toast); // إزالته من القائمة
                RepositionToasts();          // جعل الإشعارات المتبقية تنزلق للأسفل

                titleFont.Dispose();
                messageFont.Dispose();
                toast.Dispose();
            };

            toast.Show();

            try
            {
                // Fade In
                for (double i = 0.0; i <= 0.95; i += 0.1)
                {
                    toast.Opacity = i;
                    await Task.Delay(15);
                }

                await Task.Delay(duration);

                // Fade Out
                for (double i = 0.95; i >= 0.0; i -= 0.1)
                {
                    toast.Opacity = i;
                    await Task.Delay(15);
                }
            }
            finally
            {
                if (!toast.IsDisposed)
                {
                    toast.Close();
                }
            }
        }

        // 4. المحرك الهندسي لحساب مواقع الإشعارات (The Stacker Logic)
        private static void RepositionToasts()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            int marginX = 20;
            int marginY = 20;
            int spacing = 10; // المسافة بين كل إشعار والذي يليه

            // نبدأ الحساب من أسفل الشاشة
            int currentY = screen.Bottom - marginY;

            // نمر على الإشعارات من الأحدث إلى الأقدم 
            // لكي يظهر الإشعار الجديد في الأسفل، ويدفع القديم للأعلى
            for (int i = _activeToasts.Count - 1; i >= 0; i--)
            {
                var t = _activeToasts[i];

                // خصم ارتفاع الإشعار من الموقع الحالي
                currentY -= t.Height;

                // تحديث موقع الإشعار (سيتحرك فوراً للمكان الجديد)
                t.Location = new Point(screen.Right - t.Width - marginX, currentY);

                // خصم مسافة التباعد للإشعار التالي
                currentY -= spacing;
            }
        }
        #endregion
    }
}
