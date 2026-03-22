using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace CodeGeneratorSolution.Templetes.Infrastructure.Extensions
{
    public static class DataReaderExtensions
    {
        public static T GetSafe<T>(this IDataReader reader, string Name)
        {
            object value = reader[Name];
            if (value == DBNull.Value || value == null) return default;

            Type t = typeof(T);
            Type? u = Nullable.GetUnderlyingType(t);

            // If T is int?, convert to int. If T is int, convert to int.
            return (T)Convert.ChangeType(value, u ?? t);
        }
    }
}