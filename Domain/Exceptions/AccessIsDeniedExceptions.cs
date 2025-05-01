namespace Domain.Exceptions
{
    public class AccessIsDeniedException : Exception
    {
        public AccessIsDeniedException() { }

        public AccessIsDeniedException(string message) : base(message) { }
    }
}
