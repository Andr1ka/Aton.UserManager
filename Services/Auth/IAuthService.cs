using Domain;

namespace Services.Auth
{
    public interface IAuthService
    {
        Task<(bool success, string token)> AuthenticateAsync(string login, string password);
        string GenerateJwtToken(User user);
    }
} 