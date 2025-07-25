using System.Reflection;
using PortableXNB;
using XNBStudioGUI.Editors;

namespace XNBStudioGUI;

public static class PriorityResolver
{
    private static readonly Dictionary<Type, IXnbEditor> _editors = [];
    
    public static void LoadEditorsFromInterface()
    {
        var editors = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IXnbEditor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IXnbEditor>();

        foreach (var e in editors)
        {
            if(e != null)
                _editors[e.GetType()] = e;
        }
    }

    public static Dictionary<IXnbEditor, int> GetPriorityListForType(string type)
    {
        var d = new Dictionary<IXnbEditor, int>();
        foreach (var (editorType, editorInst) in _editors)
        {
            var p = editorInst.GetPriorityForType(type);
            if(p < 0) continue;
            d[editorInst] = p;
        }

        return d;
    }

    public static IXnbEditor[] GetEditorsForType(string type)
    {
        return GetPriorityListForType(type)
            .OrderByDescending(p => p.Value)
            .ToDictionary()
            .Keys.ToArray();
    }

    public static IXnbEditor[] GetEditorsForFile(XnbFile file)
    {
        var d = new Dictionary<IXnbEditor, int>();
        foreach (var priorities in file.TypeReaders.Keys.Select(type => GetPriorityListForType(type)))
        {
            d.AddFrom(priorities);
        }
        
        return d.OrderByDescending(p => p.Value)
                .ToDictionary()
                .Keys.ToArray();
    }
}
