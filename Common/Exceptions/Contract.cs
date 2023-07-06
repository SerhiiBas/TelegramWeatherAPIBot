namespace Common.Exceptions
{
    public class Contract
    {
        public static void NotNull(object obj, string message)
        {
            if (obj == null) throw new ArgumentNullException(message);
        }
    }
}