using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;

namespace IntroSEProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private AppDbContext context;
        private IMapper mapper;
        private ITokenManager tokenManager;
        private readonly IEmailSender emailService;

        public UserController(AppDbContext context, 
            IMapper mapper, 
            ITokenManager tokenManager,
            IEmailSender emailService)
        {
            this.context = context;
            this.mapper = mapper;
            this.tokenManager = tokenManager;
            this.emailService = emailService;
        }
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var u = await context.Users.FirstOrDefaultAsync(x => x.Email == registerModel.Email);
            if (u != null) return BadRequest(new { message = "The user has existed" });
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);
                registerModel.Password = passwordHash;
                User user = mapper.Map<User>(registerModel);

                await context.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("/authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Credential credential)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Email == credential.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(credential.Password, user.Password))
            {
                (string accessToken, DateTime accessTokenExpiresAt) = tokenManager.CreateAccessToken(user);
                (string refreshToken, DateTime refreshTokenExpiresAt) = tokenManager.CreateRefreshToken(user);
                SetRefreshTokenCookie(refreshToken, refreshTokenExpiresAt);

                return Ok(new {
                    user = mapper.Map<LoginModel>(user),    
                    accessToken
                });
            }

            return Unauthorized(new { message = "Email or password is incorrect" });
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpGet]
        [Route("/refresh-token")]
        [AllowAnonymous]
        public IActionResult RefreshToken()
        {
            string? value = Request.Headers["Cookie"];
            if (value == null)
            {
                return BadRequest();
            }
            string RefreshToken = value.Split("=")[1];
            (string accessToken, DateTime accessTokenExpiresAt) =
                tokenManager.ValidateRefreshToken(RefreshToken);

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest();
            }
            //SetAccessTokenCookie(accessToken, accessTokenExpiresAt);
            return Ok(new
            {
                accessToken
            });
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            // Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token", new CookieOptions { 
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok();    
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0, string keyword = "")
        {
            IEnumerable<User> list;
            if (string.IsNullOrEmpty(keyword))
            {
                list = await context.Users.ToListAsync();
            }
            else
            {
                list = await context.Users.Where(x => x.FullName.Contains(keyword)).ToListAsync();
            }
            if (per_page == 0)
            {
                per_page = list.Count();
            }

            var pager = new Pager<User>(list, page, per_page);
            return Ok(pager);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] User model)
        //{
        //    var user = await context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        await context.SaveChangesAsync();
        //        return Ok(user);
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!context.Users.Any(e => e.Id == id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }
        //}

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append("refresh_token", token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expiresAt,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] User model)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;
                user.Gender = model.Gender;
                user.DateOfBirth = model.DateOfBirth;
                user.Avatar = model.Avatar;
                await context.SaveChangesAsync();
                return Ok(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }
    }
}
