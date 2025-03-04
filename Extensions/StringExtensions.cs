using System.Text.RegularExpressions;

namespace DSEConETL.Extensions;

public static class StringExtensions
{
    public static string XmlRemoveEmptyTags(this string str)
    {
        str = Regex.Replace(str, @"<(\w+)></\1>", ""); // Remove empty elements
        return str;
    }
}