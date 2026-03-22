
using CodeGeneratorSolution.EmbeddedResources.UI;
using CodeGeneratorSolution.EmbeddedResources.UI.Helpers; // Where your generic frmHostDialog lives
using CodeGeneratorSolution.EmbeddedResources.UI.Interfaces;
using CodeGeneratorSolution.UI.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGeneratorSolution.EmbeddedResources.UI.GenericForms
{
    public partial class frmGenericManage : Form
    {
        private readonly IContentList _contentList;
        private readonly INavigationService _navService;

        public frmGenericManage(IContentList contentList, INavigationService navService)
        {
            InitializeComponent();
            _contentList = contentList;
            _navService = navService;

            // Dock the control
            var uiControl = _contentList.AsUserControl();
            uiControl.Dock = DockStyle.Fill;
            this.Controls.Add(uiControl);

            // 2. WIRE UP THE DECOUPLED EVENTS
            this.Load += frmManage_Load;
            _contentList.OnAddNewRequested += CtrlList_OnAddNewRequested;
            _contentList.OnEditRequested += CtrlList_OnEditRequested;
            _contentList.OnDetailsRequested += CtrlList_OnDetailsRequested;

            // 3. FORM STYLING
            this.Text = $"Manage {_contentList.WindowTitle}s";
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
            await _contentList.LoadDataAsync();
        }

    // ====================================================
    // EVENT HANDLERS (The Bridge)
    // ====================================================

    private async void CtrlList_OnAddNewRequested(object sender, EventArgs e)
    {
            _navService.OpenAddNew(_contentList.EntityDtoType);
             // 3. Refresh the DataGridView automatically just in case they saved a new record!
             await _contentList.LoadDataAsync();
    }

    private async void CtrlList_OnEditRequested(object sender,DtoEventArgs e)
    {
        _navService.OpenAddEdit(e.SelectedDto);
        // Refresh the grid to show the updated data
        await _contentList.LoadDataAsync();
    }

    private async void CtrlList_OnDetailsRequested(object sender, DtoEventArgs e)
        {
            _navService.ShowDetailsAsync(e.SelectedDto);
            // Refresh the grid to show the updated data
            await _contentList.LoadDataAsync();
        }
}
}