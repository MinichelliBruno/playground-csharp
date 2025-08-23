using System.Reflection;

namespace ReflectionLab;

public static class RangeIntValidation
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
    public static List<string> ValidateRangeInt(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        
        var errors = new List<string>();
        
        var type = obj.GetType();
        
        var fields = type.GetFields(Flags);

        foreach (var field in fields)
        {
            var rangeInt = field.GetCustomAttribute<RangeIntAttribute>();
            if (rangeInt == null || field.FieldType != typeof(int))
            {
                continue;
            }
            var min = rangeInt.Min;
            var max = rangeInt.Max;
            var value = (int)(field.GetValue(obj) ?? 0);

            if (value < min || value > max)
            {
                errors.Add($"{field.Name} must be between {min} and {max}");
            }
        }

        var properties = type.GetProperties(Flags);
        
        foreach (var property in properties)
        {
            var rangeInt = property.GetCustomAttribute<RangeIntAttribute>();
            if (rangeInt == null || property.PropertyType != typeof(int))
            {
                continue;
            }

            var min = rangeInt.Min;
            var max = rangeInt.Max;
            var value = (int)(property.GetValue(obj) ?? 0);

            if (value < min || value > max)
            {
                errors.Add($"{property.Name} must be between {min} and {max}");
            }
        }

        return errors;
    }
}