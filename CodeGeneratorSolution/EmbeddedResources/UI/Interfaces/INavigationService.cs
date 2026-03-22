using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGeneratorSolution.EmbeddedResources.Core.DTOs.Interfaces;



namespace CodeGeneratorSolution.EmbeddedResources.UI.Interfaces
{
    public interface INavigationService
    {
        Task ShowDetailsAsync(IBaseDto dto);
        void OpenAddEdit(IBaseDto gridDto);
        void OpenAddNew(Type entityDtoTyp);
    }
}
