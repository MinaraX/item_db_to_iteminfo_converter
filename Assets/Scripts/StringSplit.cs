using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringSplit
{
    public static List<string> GetStringSplit(string data, char targetToSplit)
    {
        if (string.IsNullOrEmpty(data))
            return new List<string>("".Split(targetToSplit));
        else
            return new List<string>(data.Split(targetToSplit));
    }

    public static List<string> GetStringSplitAll(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new List<string>("".Split());
        else
            return new List<string>(data.Split());
    }
}
