using System.Linq;

namespace GloryBot.Extensions;

public static class DictionaryExtension
{
    public static void AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> data, TKey key, TValue value)
    {
        if (!data.ContainsKey(key))
            data.Add(key, value);
    }

   

}