using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IntroSEProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Layer.Domain.Interfaces
{
	public interface ITokenManager
	{
		(string, DateTime) CreateAccessToken(User user);
		(string, DateTime) CreateRefreshToken(User user);
		Task ValidateAccessToken(TokenValidatedContext context);
		(string, DateTime) ValidateRefreshToken(string refreshToken);
	}
}
