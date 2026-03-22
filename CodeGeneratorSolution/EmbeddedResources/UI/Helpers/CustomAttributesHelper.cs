using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.UI.Helpers
{
    public static class CustomAttributesHelper
    {
        public static string GetDisplayName<T>(string propertyName)
        {
            var prop = typeof(T).GetProperty(propertyName);
            if (prop == null) return propertyName;

            var attribute = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                .FirstOrDefault() as DisplayNameAttribute;

            // Return the DisplayName if it exists, otherwise just fall back to the property name
            return attribute?.DisplayName ?? propertyName;
        }
    }
}
