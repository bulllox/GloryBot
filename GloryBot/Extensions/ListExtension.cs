using System.Linq;

namespace GloryBot.Extensions;

public static class ListExtension
{

    public static void AddIfNotExists<T>(this List<T> list, T value)
    {
        CheckListIsNull(list);
        if (!list.Contains(value))
            list.Add(value);
    }
    public static bool StringHasChar(this List<string> list, string Value)
    {
        var res = false;
        list.ForEach((string element) =>
        {
            if (element.Contains(Value))
            {
                res = true;
            }
        });
        return res;
    }
    public static void UpdateValue<T>(this IList<T> list, T value, T newValue)
    {
        CheckListAndValueIsNull(list, value);
        CheckValueIsNull(newValue);
        var index = list.IndexOf(value);
        list[index] = newValue;
    }

    public static void DeleteIfExists<T>(this IList<T> list, T value)
    {
        CheckListAndValueIsNull(list, value);
        if (list.Contains(value))
            list.Remove(value);
    }

    private static void CheckListAndValueIsNull<T>(this IList<T> list, T value)
    {
        CheckListIsNull(list);
        CheckValueIsNull(value);
    }

    private static void CheckValueIsNull<T>(T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
    }

    private static void CheckListIsNull<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
    }

    public static T Shift<T>(this List<T> list)
    {
        var lastItem = list.Last();
        list.Remove(lastItem);
        return lastItem;
    }
}