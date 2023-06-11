using RwaMovies.Models;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs.Auth
{
    public class RegisterRequest
    {
        public int? Id { get; set; }

        [StringLength(50, MinimumLength = 6)]
        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Phone { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
}
