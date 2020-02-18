using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringSplit
{
    public static List<string> GetStringSplit(string data, char targetToSplit)
    {
        return new List<string>(data.Split(targetToSplit));
    }
}
