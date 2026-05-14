
using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;
using System;

namespace {{TARGET_NAMESPACE}}.UI.Events
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