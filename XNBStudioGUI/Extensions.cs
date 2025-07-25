namespace XNBStudioGUI;

public static class Extensions
{
    public static void AddFrom<TK, TV>(this Dictionary<TK, TV> dest, Dictionary<TK, TV> src, bool replace = false) where TK : notnull
    {
        foreach (var (key, val) in src)
        {
            if (dest.TryAdd(key, val)) continue;
            
            // couldn't be added
            if (replace)
            {
                dest[key] = val;
            }
        }
    }
}