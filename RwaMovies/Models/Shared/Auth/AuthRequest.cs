using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared.Auth
{
    public class AuthRequest
    {
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
