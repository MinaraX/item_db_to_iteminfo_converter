using System.Text.RegularExpressions;

/// <summary>
/// Credit: https://stackoverflow.com/questions/13870725/how-to-search-and-replace-exact-matching-strings-only
/// </summary>
public static class StringExtensions
{
    public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
    {
        string searchString = find.StartsWith(".@") ? $@".@\b{find.Substring(2)}\b" : $@"\b{find}\b";
        string textToFind = matchWholeWord ? searchString : find;
        return Regex.Replace(input, textToFind, replace);
    }

    public static bool IsSafeContains(this string input, string find)
    {
        var s = input;

        string searchString = find.StartsWith(".@") ? $@".@\b{find.Substring(2)}\b" : $@"\b{find}\b";
        s = Regex.Replace(s, searchString, "[FOUND]");

        return s == input ? false : true;
    }
}