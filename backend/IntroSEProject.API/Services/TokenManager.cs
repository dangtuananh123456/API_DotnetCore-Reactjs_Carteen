using IntroSEProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntroSEProject.API.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IConfiguration configuration;
        private readonly AppDbContext dbContext;

        public TokenManager(IConfiguration configuration, AppDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }
        public (string, DateTime) CreateAccessToken(User user)
        {
            var issuer = configuration.GetValue<string>("AuthToken:Issuer");
            var audience = configuration.GetValue<string>("AuthToken:Audience");
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("AuthToken:SecretKey") ?? "");
            var expiresAt = DateTime.UtcNow.AddHours(1);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, issuer),
                new Claim(JwtRegisteredClaimNames.Iss, issuer, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiresAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSecurityToken = new JwtSecurityToken(
              issuer: issuer,
              audience: audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: expiresAt,
              signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
              )
            );
            return (new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), expiresAt);
        }

        public (string, DateTime) CreateRefreshToken(User user)
        {
            var issuer = configuration.GetValue<string>("AuthToken:Issuer");
            var audience = configuration.GetValue<string>("AuthToken:Audience");
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("AuthToken:SecretKey") ?? "");
            var expiresAt = DateTime.UtcNow.AddDays(60);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, issuer),
                new Claim(JwtRegisteredClaimNames.Iss, issuer, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiresAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, user.Email),
            };
           
            var jwtSecurityToken = new JwtSecurityToken(
              issuer: issuer,
              audience: audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: expiresAt,
              signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
              )
            );
            return (new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), expiresAt);
        }

        public async Task ValidateAccessToken(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();

            if (claims.Count == 0)
            {
                context.Fail("This token has no claim");
                return;
            }

            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity.FindFirst(ClaimTypes.Email) != null)
            {
                var email = identity.FindFirst(ClaimTypes.Email).Value;
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    context.Fail("Invalid token");
                    return;
                }
            }

            if (identity.FindFirst(JwtRegisteredClaimNames.Exp) == null)
            {
                var expiryDate = identity.FindFirst(JwtRegisteredClaimNames.Exp).Value;
                var date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryDate)).DateTime;
                var minutes = date.Subtract(DateTime.UtcNow).TotalMinutes;
                if (minutes < 0)
                {
                    context.Fail("The token is expired");
                    return;
                }
            }

        }

        public (string, DateTime) ValidateRefreshToken(string refreshToken)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("AuthToken:SecretKey") ?? "");
            var claimPrincipal = new JwtSecurityTokenHandler().ValidateToken(
              refreshToken,
              new TokenValidationParameters()
              {
                  RequireExpirationTime = true,
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                  ValidateAudience = true,
                  ValidateIssuer = true,
                  ValidateLifetime = true,
                  ValidIssuer = configuration.GetValue<string>("AuthToken:Issuer"),
                  ValidAudience = configuration.GetValue<string>("AuthToken:Audience"),
                  ClockSkew = TimeSpan.Zero
              }, out _
            );
            if (claimPrincipal == null) return ("", default);
            var email = claimPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return ("", default);
            var user = dbContext.Users.SingleOrDefault(x => x.Email == email);  

            return CreateAccessToken(user);
        }

    }
}
