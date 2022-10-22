using System.Text.RegularExpressions;

namespace GloryBot.Extensions;

public static class StringExtension
{

    public static int ToInt(this string str) => int.Parse(str);

    public static bool ToBoolean(this string str) => Convert.ToBoolean(str);
    public static float ToFloat(this string str) => float.Parse(str);

    public static String PregReplace(this String input, string[] pattern, string[] replacements)
    {
        if (replacements.Length != pattern.Length)
            throw new ArgumentException("Replacement and Pattern Arrays must be balanced");

        for (var i = 0; i < pattern.Length; i++)
        {
            input = Regex.Replace(input, pattern[i], replacements[i]);
        }

        return input;
    }

    public static string FirstLetterToUpper(this string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    public static string Last(this string[] stringArray)
        => stringArray[stringArray.Length - 1];

    public static List<string> ToStringList(this string[] strArray)
    {
        List<string> list = new();
        foreach (var str in strArray)
        {
            if (!list.Contains(str))
            {
                list.Add(str);
            }
        }
        return list;
    }

    public static string UcFirst(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };

}