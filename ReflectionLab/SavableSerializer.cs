using System.Collections.Concurrent;
using System.Reflection;

namespace ReflectionLab;

public static class SavableSerializer
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static ConcurrentDictionary<Type, MemberAccessor[]> _cache = new();

    public static IReadOnlyDictionary<string, object?> Serialize(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException();
        }

        var type = obj.GetType();
        var accessors = GetMemberAccessors(type);

        _cache.GetOrAdd(type, accessors);
        

        var dict = new Dictionary<string, object?>(accessors.Length,  StringComparer.Ordinal);
        
        foreach (var accessor in accessors)
        {
            dict.Add(accessor.Name, accessor.Getter(obj));
        }

        return dict;
    }

    private static MemberAccessor[] GetMemberAccessors(Type type)
    {
        List<MemberAccessor> accessors = new();
        var fields = type.GetFields(Flags);

        foreach (var field in fields)
        {
            var savable = field.GetCustomAttribute<SavableAttribute>();
            if (savable == null)
            {
                continue;
            }
            
            var fieldMemberAccessor = new MemberAccessor(savable.Name ?? field.Name, o => field.GetValue(o));
            accessors.Add( fieldMemberAccessor );
        }
        
        var properties = type.GetProperties(Flags);
        foreach (var property in properties)
        {
            if (property.GetIndexParameters().Length > 0)
            {
                continue;
            }
            var savable = property.GetCustomAttribute<SavableAttribute>();
            if (savable == null)
            {
                continue;
            }
            var propertyMemberAccessor = new MemberAccessor(savable.Name ?? property.Name, o => property.GetValue(o));
            accessors.Add( propertyMemberAccessor );
        }

        return accessors.ToArray();
    }

    private sealed class MemberAccessor(string name, Func<object, object?> getter)
    {
        public string Name { get; } = name;
        public Func<object, object?> Getter { get; } = getter;
    }
}