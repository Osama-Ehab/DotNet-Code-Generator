
using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces.Controls;
using CodeGeneratorSolution.UI;
using CodeGeneratorSolution.UI.Events;
using CodeGeneratorSolution.UI.Helpers; // Where your generic frmHostDialog lives
using CodeGeneratorSolution.UI.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGeneratorSolution.UI.GenericForms
{
    public partial class frmGenericManage : Form
    {
        private readonly IListControlWithNotifies _listControl;
        private readonly INavigationService _navService;

        public frmGenericManage(IListControlWithNotifies listControl, INavigationService navService)
        {
            InitializeComponent();
            _listControl = listControl;
            _navService = navService;

            // Dock the control
            var uiControl = _listControl.AsUserControl();
            uiControl.Dock = DockStyle.Fill;
            this.Controls.Add(uiControl);

            // 2. WIRE UP THE DECOUPLED EVENTS
            this.Load += frmManage_Load;
            _listControl.AddNew += CtrlList_OnAddNew;
            _listControl.Edit += CtrlList_OnEdit;
            _listControl.ShowDetails += CtrlList_OnShowDetails;

            // 3. FORM STYLING
            this.Text = $"Manage {_listControl.Title}s";
            this.Size = new System.Drawing.Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Apply the matching icon to the Form window!
            //this.Icon = ResourceImageHelper.GetEntityIconAsIcon();
        }

        // ====================================================
        // LOAD DATA ON STARTUP
        // ====================================================
        private async void frmManage_Load(object sender, EventArgs e)
        {
            await _listControl.LoadDataAsync();
        }

    // ====================================================
    // EVENT HANDLERS (The Bridge)
    // ====================================================

    private async void CtrlList_OnAddNew(object sender, EventArgs e)
    {
            _navService.OpenAddNew(_listControl.EntityDtoType);
             // 3. Refresh the DataGridView automatically just in case they saved a new record!
             await _listControl.LoadDataAsync();
    }

    private async void CtrlList_OnEdit(object sender,DtoEventArgs e)
    {
        _navService.OpenAddEdit(e.SelectedDto);
        // Refresh the grid to show the updated data
        await _listControl.LoadDataAsync();
    }

    private async void CtrlList_OnShowDetails(object sender, DtoEventArgs e)
        {
            _navService.ShowDetailsAsync(e.SelectedDto);
            // Refresh the grid to show the updated data
            await _listControl.LoadDataAsync();
        }
}
}