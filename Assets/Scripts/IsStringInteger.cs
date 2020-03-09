public class IsStringInteger
{
    public static bool Check(string data)
    {
        //Integer check
        int paramInt = 0;
        bool isInteger = false;

        isInteger = int.TryParse(data, out paramInt);

        return isInteger;
    }
}
