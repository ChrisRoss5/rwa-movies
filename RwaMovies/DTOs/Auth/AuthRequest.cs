using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs.Auth
{
    public class AuthRequest
    {
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
