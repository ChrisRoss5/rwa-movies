using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Web;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared.Auth;

namespace RwaMovies.Services
{
    public interface IAuthService
    {
        Task Register(UserRequest userRequest);
        Task ConfirmEmail(string username, string b64SecToken);
        Task<User> GetUser(AuthRequest authRequest);
        string GetRole(User user);
        Task<string> GetJwtToken(AuthRequest authRequest);
        Task ChangePassword(NewPasswordRequest newPasswordRequest);
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

        public async Task Register(UserRequest userRequest)
        {
            if (userRequest.Password1 != userRequest.Password2)
                throw new InvalidOperationException("Passwords do not match");
            if (await _context.Users.AnyAsync(
                u => u.Username.ToLower() == userRequest.Username.ToLower().Trim()))
                throw new InvalidOperationException("Username already exists");
            var user = _mapper.Map<User>(userRequest);
            if (!userRequest.IsConfirmed)
            {
                var confirmUrl = $"{_configuration["AppUrl"]}/Auth/ConfirmEmail?" +
                    $"username={HttpUtility.UrlEncode(user.Username)}&" +
                    $"b64SecToken={HttpUtility.UrlEncode(user.SecurityToken)}";
                var notification = new Notification
                {
                    CreatedAt = DateTime.UtcNow,
                    ReceiverEmail = userRequest.Email,
                    Subject = "Confirm your email",
                    Body = $"Hi {user.Username}, please click <a href=\"{confirmUrl}\">here</a> " +
                        $"to confirm your email and complete registration."
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

        public async Task<User> GetUser(AuthRequest authRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authRequest.Username);
            if (user == null)
                throw new InvalidOperationException("Incorrect username or password");
            byte[] salt = Convert.FromBase64String(user.PwdSalt);
            byte[] hash = Convert.FromBase64String(user.PwdHash);
            byte[] calcHash = AuthUtils.HashPassword(authRequest.Password, salt);
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

        public async Task<string> GetJwtToken(AuthRequest authRequest)
        {
            var user = await GetUser(authRequest);
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
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);
            return serializedToken;
        }

        public async Task ChangePassword(NewPasswordRequest newPasswordRequest)
        {
            if (newPasswordRequest.NewPassword1 != newPasswordRequest.NewPassword2)
                throw new InvalidOperationException("Passwords don't match");
            var user = await GetUser(newPasswordRequest.AuthRequest);
            byte[] salt = AuthUtils.GenerateSalt();
            user.PwdSalt = Convert.ToBase64String(salt);
            user.PwdHash = Convert.ToBase64String(
                AuthUtils.HashPassword(newPasswordRequest.NewPassword1, salt));
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
