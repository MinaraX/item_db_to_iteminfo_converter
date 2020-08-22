using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThaiNumber
{
    public static string IntToThai(int num)
    {
        if (num == 0) return "๐";
        else if (num == 1) return "๑";
        else if (num == 2) return "๒";
        else if (num == 3) return "๓";
        else if (num == 4) return "๔";
        else if (num == 5) return "๕";
        else if (num == 6) return "๖";
        else if (num == 7) return "๗";
        else if (num == 8) return "๘";
        else if (num == 9) return "๙";
        return null;
    }

    public static string ThaiToInt(string data)
    {
        data = data.Replace("๐", "0");
        data = data.Replace("๑", "1");
        data = data.Replace("๒", "2");
        data = data.Replace("๓", "3");
        data = data.Replace("๔", "4");
        data = data.Replace("๕", "5");
        data = data.Replace("๖", "6");
        data = data.Replace("๗", "7");
        data = data.Replace("๘", "8");
        data = data.Replace("๙", "9");
        return data;
    }
}
