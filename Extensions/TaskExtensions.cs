namespace FCMicroservices.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> AsTask<T>(this T given)
        {
            return Task.FromResult(given);
        }
    }
}
