using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// Credit: https://www.techiedelight.com/remove-whitespace-from-string-csharp/
/// </summary>

public class MergeWhiteSpace : MonoBehaviour
{
    static Regex regex = new Regex(@"\s+");
    public static string RemoveWhiteSpace(string str)
    {
        return regex.Replace(str, string.Empty);
    }
}