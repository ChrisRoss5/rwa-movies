using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs.Auth
{
    public class UserRequest
    {
        public int? Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;
        [DisplayName("Password")]
        public string Password1 { get; set; } = null!;
        [DisplayName("Confirm password")]
        public string Password2 { get; set; } = null!;
        [DisplayName("First name")]
        public string FirstName { get; set; } = null!;
        [DisplayName("Last name")]
        public string LastName { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Phone]
        public string? Phone { get; set; }
        [DisplayName("Is confirmed")]
        public bool IsConfirmed { get; set; }
        [DisplayName("Country of residence")]
        public int CountryOfResidenceId { get; set; }
    }
}
