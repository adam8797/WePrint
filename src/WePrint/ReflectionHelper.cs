using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WePrint
{
    public static class ReflectionHelper
    {
        public static void Update<TFrom, TTo>(TFrom source, TTo destination)
        {
            Update(source, typeof(TFrom), destination, typeof(TTo));
        }

        private static void Update(object from, Type tFrom, object to, Type tto)
        {
            foreach (var property in tFrom.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                if (property.GetCustomAttribute<DontUpdateAttribute>() != null || property.GetCustomAttribute<KeyAttribute>() != null)
                    continue;

                if(property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && property.PropertyType.GetGenericArguments()[0] == property.PropertyType)
                {
                    // We're assigning nullable -> non nullable. Ignore the copy if the source value is null, copy it otherwise
                    var fromValue = property.GetValue(from);
                    if(fromValue != null)
                    {
                        property.SetValue(to, fromValue);
                    }
                }
                else if(property.PropertyType != property.PropertyType)
                {
                    // Mismatched types, try to copy recursively
                    object recursiveTarget = property.GetValue(to);
                    Update(property.GetValue(from), property.PropertyType, recursiveTarget, property.PropertyType);
                    property.SetValue(to, recursiveTarget);
                }
                else
                {
                    // Types matched, shallow copy non-null values
                    var fromValue = property.GetValue(from);
                    if (fromValue != null)
                    {
                        property.SetValue(to, fromValue);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DontUpdateAttribute : Attribute
    {
    }
}
