
using CodeGeneratorSolution.EmbeddedResources.Core.DTOs.Interfaces;
using System;

namespace CodeGeneratorSolution.UI.Events
{
    public class DtoEventArgs : EventArgs
    {
        public IBaseDto SelectedDto { get; }

        public DtoEventArgs(IBaseDto selectedDto)
        {
            SelectedDto = selectedDto;
        }
    }
}