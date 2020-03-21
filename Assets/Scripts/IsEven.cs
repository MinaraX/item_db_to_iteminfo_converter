public class IsEven
{
    public static bool IsCircleEven(string txt)
    {
        int a = 0;
        int b = 0;

        foreach (char c in txt)
        {
            if (c == '(')
                a++;
            else if (c == ')')
                b++;
        }

        if (a != b)
            return false;
        else
            return true;
    }

    public static bool IsCurlyEven(string txt)
    {
        int a = 0;
        int b = 0;

        foreach (char c in txt)
        {
            if (c == '{')
                a++;
            else if (c == '}')
                b++;
        }

        if (a != b)
            return false;
        else
            return true;
    }
}
