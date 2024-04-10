using IntroSEProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IntroSEProject.API.Services
{
    public interface ITokenManager
    {
        (string, DateTime) CreateAccessToken(User user);
        (string, DateTime) CreateRefreshToken(User user);
        Task ValidateAccessToken(TokenValidatedContext context);
        (string, DateTime) ValidateRefreshToken(string refreshToken);
    }
}