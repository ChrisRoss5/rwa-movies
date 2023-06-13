using System.ComponentModel;

namespace RwaMovies.DTOs.Auth
{
    public class AuthRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
