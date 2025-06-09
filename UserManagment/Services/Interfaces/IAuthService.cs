namespace UserManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(string username, string password);
        Task<string> RefreshTokenAsync(string refreshToken);
    }
}
