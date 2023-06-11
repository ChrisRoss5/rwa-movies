using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RwaMovies.Models;
using RwaMovies.DTOs.Auth;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace RwaMovies.Services
{
    public interface IUsersService
    {
        Task<RegisterResponse> Register(RegisterRequest request);
        Task ValidateEmail(ValidateEmailRequest request);
        Task<Tokens> JwtTokens(LoginRequest request);
        Task ChangePassword(ChangePasswordRequest request);
    }
    public class UsersService : IUsersService
    {
        private readonly IConfiguration _configuration;
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public UsersService(RwaMoviesContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var normalizedUsername = request.Username.ToLower().Trim();
            if (await _context.Users.AnyAsync(x => x.Username.ToLower() == normalizedUsername))
                throw new InvalidOperationException("Username already exists");
            var newUser = _mapper.Map<User>(request);
            _context.Users.Add(newUser);
            _context.Notifications.Add(new Notification
            {
                CreatedAt = DateTime.UtcNow,
                ReceiverEmail = request.Email,
                Subject = "Confirm your email — RwaMovies",
                Body = $"Click <a href=\"https://localhost:7116/users/validate-email?username={normalizedUsername}&b64SecToken={newUser.SecurityToken}\">here</a> to confirm your email."
            }); /*{_configuration["AppUrl"]}*/
            await _context.SaveChangesAsync();
            return _mapper.Map<RegisterResponse>(newUser);
        }

        public async Task ValidateEmail(ValidateEmailRequest request)
        {
            var target = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == request.Username && u.SecurityToken == request.B64SecToken);
            if (target == null)
                throw new InvalidOperationException("Authentication failed");
            target.IsConfirmed = true;
            _context.Entry(target).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        private async Task<bool> Authenticate(string username, string password)
        {
            var target = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (target == null || !target.IsConfirmed)
                return false;
            byte[] salt = Convert.FromBase64String(target.PwdSalt);
            byte[] hash = Convert.FromBase64String(target.PwdHash);
            byte[] calcHash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            return hash.SequenceEqual(calcHash);
        }

        public async Task<string> GetRole(string username)
        {
            var target = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (target == null)
                throw new InvalidOperationException("Username does not exist");
            return username == "admin" ? "admin" : "user"; 
        }

        public async Task<Tokens> JwtTokens(LoginRequest request)
        {
            var isAuthenticated = await Authenticate(request.Username, request.Password);
            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");
            var jwtKey = _configuration["JWT:Key"]!;
            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var role = await GetRole(request.Username);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    new Claim(ClaimTypes.Role, role)
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
            return new Tokens
            {
                Token = serializedToken
            };
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            if (request.NewPassword1 != request.NewPassword2)
                throw new InvalidOperationException("Passwords don't match");
            var isAuthenticated = await Authenticate(request.Username, request.OldPassword);
            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string b64Salt = Convert.ToBase64String(salt);
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.NewPassword1,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);
            var target = await _context.Users.FirstAsync(u => u.Username == request.Username);
            target.PwdSalt = b64Salt;
            target.PwdHash = b64Hash;
            _context.Entry(target).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
