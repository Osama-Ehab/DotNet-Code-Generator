using CodeGeneratorSolution.CSharp_Compiler.UI.Interfaces;
using CodeGeneratorSolution.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.CSharp_Compiler.UI.Controls
{
    public abstract class BaseControl<TKey> : IPopupControl, ILoadDataById
    {
        public abstract string Title { get; }
        public abstract int Width { get; }

        public abstract UserControl AsUserControl();
        public abstract void FillControl();
        public abstract Task LoadDataAsync(TKey Id);

        Task ILoadDataById.LoadDataAsync(object Id)
        {
            return LoadDataAsync((TKey) Id);
        }


    }
}
