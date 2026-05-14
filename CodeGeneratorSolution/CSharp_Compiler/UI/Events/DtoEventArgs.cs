
using CodeGeneratorSolution.Core.DTOs.Interfaces;
using System;

namespace CodeGeneratorSolution.UI.Events
{
    public class DtoEventArgs : EventArgs
    {
        public IIdentifiableDto SelectedDto { get; }

        public DtoEventArgs(IIdentifiableDto selectedDto)
        {
            SelectedDto = selectedDto;
        }
    }
}