using {{TARGET_NAMESPACE}}.UI.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Controls
{
   public abstract class BaseControl : IPopupControl
{
    public abstract string Title { get; }
    public abstract int Width { get; }

    public abstract UserControl AsUserControl();
    public abstract void FillControl();
    public abstract Task LoadDataAsync();

}
public abstract class BaseControl<TKey> : BaseControl, ILoadDataById
{
    public abstract Task LoadDataAsync(TKey Id);

    Task ILoadDataById.LoadDataAsync(object Id)
    {
        return LoadDataAsync((TKey)Id);
    }


}
}
