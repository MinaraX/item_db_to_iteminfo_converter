using System.Collections.Generic;
using System;

public class StringSplit
{
    public static List<string> GetStringSplit(string data, char targetToSplit)
    {
        if (string.IsNullOrEmpty(data))
            return new List<string>("".Split(targetToSplit));
        else
            return new List<string>(data.Split(targetToSplit));
    }

    public static List<string> GetStringSplitMoreThanOneChar(string data, string[] targetToSplit)
    {
        if (string.IsNullOrEmpty(data))
            return new List<string>("".Split(' '));
        else
            return new List<string>(data.Split(targetToSplit, StringSplitOptions.None));
    }
}
