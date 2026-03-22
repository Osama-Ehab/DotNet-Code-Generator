using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Utlis
{
    public static class NameFormatter
    {
        public static string Singularize(this string value)
        {
            return value.Singularize(false);
        }
        public static string ToPascalCase(this string value)
        {
            return value.Pascalize();
        }

    }
}
