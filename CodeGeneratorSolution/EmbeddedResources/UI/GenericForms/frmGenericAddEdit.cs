
using CodeGeneratorSolution.EmbeddedResources.UI.Interfaces;
using CodeGeneratorSolution.EmbeddedResources.UI.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGeneratorSolution.EmbeddedResources.UI.GenericForms
{
    public partial class frmGenericAddEdit : Form
    {
        private readonly ISaveableControl _saveableContent;

        // Hold a class-level reference to the Layout Engine
        private readonly TableLayoutPanel _mainLayout;

        public frmGenericAddEdit(ISaveableControl saveable)
        {
            InitializeComponent();

            _saveableContent = saveable;

            // 2. Setup Form
            this.Text = _saveableContent.WindowTitle; // Dynamic Title
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ControlBox = false; // Hide X button to force use of Cancel

            // 2. Initialize TableLayoutPanel Engine
            _mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 4, // 0: Header, 1: Main, 2: Bottom, 3: Buttons
                Padding = new Padding(10)
            };

            // 3. Define Strict Row Constraints
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 0: Header
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 1: Main Content
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 2: Bottom Control
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Row 3: Buttons (Fixed Height)

            this.Controls.Add(_mainLayout);

            // 4. Add Main Content (Hardcoded to Row 1)
            var ctrl = saveable.AsUserControl();
            ctrl.Dock = DockStyle.Fill;
            ctrl.Margin = new Padding(0, 0, 0, 15);
            _mainLayout.Controls.Add(ctrl, 0, 1);


            // 2. THE MAGIC: Ask the control if it has a Header or Footer!
            if (_saveableContent.HeaderControl != null)
            {
                AddHeaderControl(_saveableContent.HeaderControl);
            }

            if (_saveableContent.BottomControl != null)
            {
                AddBottomControl(_saveableContent.BottomControl);
            }

            // 5. Add Buttons (Hardcoded to Row 3)
            var btnPanel = CreateButtonPanel(ctrl.Width);
            _mainLayout.Controls.Add(btnPanel, 0, 3);
            // 2. SUBSCRIBE TO SMART EVENTS
            SubscribeToSmartEvents();

            // 3. Ensure we unsubscribe when the form closes to prevent memory leaks!
            this.FormClosed += FrmGenericAddEdit_FormClosed;
        }

        private void SubscribeToSmartEvents()
        {
            if (_saveableContent != null)
            {
                _saveableContent.OnDataLoaded += HandleDataLoaded;
                _saveableContent.OnValidationStateChanged += HandleValidationStateChanged;
                _saveableContent.OnRequestFormClose += HandleRequestFormClose;
            }
        }

        private void FrmGenericAddEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // MEMORY LEAK PREVENTION: Unsubscribe from all events!
            if (_saveableContent != null)
            {
                _saveableContent.OnDataLoaded -= HandleDataLoaded;
                _saveableContent.OnValidationStateChanged -= HandleValidationStateChanged;
                _saveableContent.OnRequestFormClose -= HandleRequestFormClose;
            }
        }

        // ==========================================
        // EVENT HANDLERS (The "Smart" Logic)
        // ==========================================

        private void HandleDataLoaded(object sender, EventArgs e)
        {
            // Update the form's title dynamically!
            // e.g., Changes from "Loading..." to "Edit User: admin123"
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Text = _saveableContent.WindowTitle));
            }
            else
            {
                this.Text = _saveableContent.WindowTitle;
            }
        }

        private void HandleValidationStateChanged(object sender, bool isValid)
        {
            // Instantly disable/enable the Save button based on the UserControl's rules
            if (_btnSave != null && !_btnSave.IsDisposed)
            {
                _btnSave.Enabled = isValid;
            }
        }

        private void HandleRequestFormClose(object sender, EventArgs e)
        {
            // Allow the UserControl to close the form (e.g., if a record was deleted internally)
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        // ... (Keep your existing BtnSave_Click logic) ...

        public void AddHeaderControl(UserControl headerCtrl)
        {
            headerCtrl.Dock = DockStyle.Fill;
            headerCtrl.Margin = new Padding(0, 0, 0, 10);

            // Drop it exactly into Column 0, Row 0. Zero Z-index issues.
            _mainLayout.Controls.Add(headerCtrl, 0, 0);
        }

        public void AddBottomControl(UserControl bottomCtrl)
        {
            bottomCtrl.Dock = DockStyle.Fill;
            bottomCtrl.Margin = new Padding(0, 0, 0, 15);

            // Drop it exactly into Column 0, Row 2.
            _mainLayout.Controls.Add(bottomCtrl, 0, 2);
        }

        private Panel CreateButtonPanel(int width)
        {
            Panel pnl = new Panel
            {
                Width = width,
                Height = 40,
                Dock = DockStyle.Fill, // Fill the Absolute 50F Row height
                Margin = new Padding(0)
            };


            // Inside frmGenericAddEdit.CreateButtonPanel()
            if (!_saveableContent.IsReadOnly)
            {

                // SAVE BUTTON
                Button btnSave = new Button
                {
                    Text = "Save",
                    DialogResult = DialogResult.None,
                    Location = new Point(width - 80, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right // Pin to the right side
                };

                btnSave.Text = string.IsNullOrEmpty(_saveableContent.ActionButtonText) ? "Save" : _saveableContent.ActionButtonText;
                btnSave.Click += btnSave_Click;
                pnl.Controls.Add(btnSave);
                // Allow "Enter" key to trigger Save
                this.AcceptButton = btnSave;
            }

            // CANCEL BUTTON
            Button btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(width - 170, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancel.Click += (s, e) => this.Close();

            pnl.Controls.Add(btnCancel);

            this.CancelButton = btnCancel;

            return pnl;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                return;

            var btn = (Button)sender;
            btn.Enabled = false;

            try
            {
                bool success = await _saveableContent.SaveDataAsync();

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected UI/Form-level errors
                MessageServiceHelper.ShowError($"An error occurred while saving: {ex.Message}");
            }
            finally
            {
                // Safety check in case the form is already disposed
                if (!this.IsDisposed && !btn.IsDisposed)
                {
                    btn.Enabled = true;
                }
            }
        }
    }
}
