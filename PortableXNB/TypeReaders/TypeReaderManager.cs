using System.Reflection;

namespace PortableXNB.TypeReaders;

public static class TypeReaderManager
{
    private static readonly Dictionary<Type, ITypeReader> _readers = [];

    static TypeReaderManager()
    {
        LoadFromInterface();
    }
    
    public static void LoadFromInterface()
    {
        var editors = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ITypeReader).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ITypeReader>();

        foreach (var e in editors)
        {
            if(e != null)
                _readers[e.GetType()] = e;
        }
    }

    public static ITypeReader? GetReaderForType(string type)
    {
        foreach (var (_, r) in _readers)
        {
            if (r.MatchType(type))
            {
                return r;
            }
        }

        return null;
    }
}