using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RwaMovies.DALModels;
using RwaMovies.SharedModels.Auth;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace RwaMovies.Services
{
    public interface IAuthService
    {
        Task Register(UserRequest request, bool isConfirmed = false);
        Task ConfirmEmail(string username, string b64SecToken);
        Task<User> GetUser(AuthRequest request);
        string GetRole(User user);
        Task<string> GetJwtToken(AuthRequest request);
        Task ChangePassword(NewPasswordRequest request);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;
        private readonly IMailService _mail;

        public AuthService(RwaMoviesContext context, IConfiguration configuration, IMapper mapper, IMailService mail)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _mail = mail;
        }

        public async Task Register(UserRequest request, bool isConfirmed = false)
        {
            if (request.Password1 != request.Password2)
                throw new InvalidOperationException("Passwords do not match");
            if (await _context.Users.AnyAsync(
                u => u.Username.ToLower() == request.Username.ToLower().Trim()))
                throw new InvalidOperationException("Username already exists");
            var user = _mapper.Map<User>(request);
            if (!isConfirmed)
            {
                var confirmUrl = $"https://localhost:7116/Auth/ConfirmEmail?" + /*{_configuration["AppUrl"]} todo */
                    $"username={HttpUtility.UrlEncode(user.Username)}&" +
                    $"b64SecToken={HttpUtility.UrlEncode(user.SecurityToken)}";
                var notification = new Notification
                {
                    CreatedAt = DateTime.UtcNow,
                    ReceiverEmail = request.Email,
                    Subject = "Confirm your email — RwaMovies",
                    Body = $"Click <a href=\"{confirmUrl}\">here</a> to confirm your email."
                };
                await _mail.Send(notification.ReceiverEmail, notification.Subject, notification.Body);
                notification.SentAt = DateTime.UtcNow;
                _context.Notifications.Add(notification);
            }
            else
                user.IsConfirmed = true;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmEmail(string username, string b64SecToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == username && u.SecurityToken == b64SecToken);
            if (user == null)
                throw new InvalidOperationException("Email validation failed");
            user.IsConfirmed = true;
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUser(AuthRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                throw new InvalidOperationException("Incorrect username or password");
            byte[] salt = Convert.FromBase64String(user.PwdSalt);
            byte[] hash = Convert.FromBase64String(user.PwdHash);
            byte[] calcHash = AuthUtils.HashPassword(request.Password, salt);
            if (!hash.SequenceEqual(calcHash))
                throw new InvalidOperationException("Incorrect username or password");
            if (user.DeletedAt != null)
                throw new InvalidOperationException("User is deactivated");
            if (!user.IsConfirmed)
                throw new InvalidOperationException("Email is not confirmed");
            return user;
        }

        public string GetRole(User user)
        {
            // The given database doesn't have a role column, so I'm just hardcoding it here
            return user.Username.ToLower() == "admin" ? "Admin" : "User";
        }

        public async Task<string> GetJwtToken(AuthRequest request)
        {
            var user = await GetUser(request);
            var jwtKey = _configuration["JWT:Key"]!;
            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(ClaimTypes.Role, GetRole(user))
                }),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);
            return serializedToken;
        }

        public async Task ChangePassword(NewPasswordRequest request)
        {
            if (request.NewPassword1 != request.NewPassword2)
                throw new InvalidOperationException("Passwords don't match");
            var user = await GetUser(request.AuthRequest);
            byte[] salt = AuthUtils.GenerateSalt();
            user.PwdSalt = Convert.ToBase64String(salt);
            user.PwdHash = Convert.ToBase64String(AuthUtils.HashPassword(request.NewPassword1, salt));
            await _context.SaveChangesAsync();
        }
    }

    public static class AuthUtils
    {
        public static byte[] GenerateSecurityToken()
        {
            return RandomNumberGenerator.GetBytes(256 / 8);
        }

        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(128 / 8);
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }
    }
}
