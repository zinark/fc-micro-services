namespace FCMicroservices.Extensions;

public static class ObjectExtensions
{
    public static T Cast<T>(this object obj)
    {
        return (T)obj;
    }
}