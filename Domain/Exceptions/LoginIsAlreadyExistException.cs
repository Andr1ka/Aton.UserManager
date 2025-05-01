namespace Services.Users
{
    public class LoginIsAlreadyExistException : Exception
    {
        public LoginIsAlreadyExistException() { }
        public LoginIsAlreadyExistException(string message) : base(message) { }
    }
}
