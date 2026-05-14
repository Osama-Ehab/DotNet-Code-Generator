using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;
using {{TARGET_NAMESPACE}}.UI.Interfaces;



namespace {{TARGET_NAMESPACE}}.UI.Interfaces
{
    public interface INavigationService : IShowDetails,IOpenAddEdit,IOpenAddNew
    {
     
    }

}
