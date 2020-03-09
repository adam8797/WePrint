using System;
using System.Reflection;

namespace WePrint.Common
{
    public static class ReflectionHelper
    {
        public static void CopyPropertiesTo<TFrom, TTo>(TFrom source, TTo destination)
        {
            CopyPropertiesTo(source, typeof(TFrom), destination, typeof(TTo));
        }

        private static void CopyPropertiesTo(object from, Type tFrom, object to, Type tto)
        {
            foreach (var property in tFrom.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                var tgtProperty = tto.GetProperty(property.Name);
                if (tgtProperty == null) 
                    // No matching property, skip this one
                    continue;
                else if(property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && property.PropertyType.GetGenericArguments()[0] == tgtProperty.PropertyType)
                {
                    // We're assigning nullable -> non nullable. Ignore the copy if the source value is null, copy it otherwise
                    var fromValue = property.GetValue(from);
                    if(fromValue != null)
                    {
                        tgtProperty.SetValue(to, fromValue);
                    }
                }
                else if(tgtProperty.PropertyType != property.PropertyType)
                {
                    // Mismatched types, try to copy recursively
                    object recursiveTarget = tgtProperty.GetValue(to);
                    CopyPropertiesTo(property.GetValue(from), property.PropertyType, recursiveTarget, tgtProperty.PropertyType);
                    tgtProperty.SetValue(to, recursiveTarget);
                }
                else
                {
                    // Types matched, shallow copy non-null values
                    var fromValue = property.GetValue(from);
                    if (fromValue != null)
                    {
                        tgtProperty.SetValue(to, fromValue);
                    }
                }
            }
        }
    }
}
