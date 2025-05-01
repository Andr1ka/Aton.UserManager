namespace Domain.Exceptions
{
    public class UserIsRevokedException : Exception
    {
        public UserIsRevokedException() { }
        public UserIsRevokedException(string message) : base(message) { }
    }
}
