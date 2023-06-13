using System.ComponentModel;

namespace RwaMovies.DTOs.Auth
{
    public class UserResponse
    {
        public int? Id { get; set; }
        [DisplayName("Created at")]
        public DateTime CreatedAt { get; set; }
        [DisplayName("Deactivated at")]
        public DateTime? DeletedAt { get; set; }
        [DisplayName("User name")]
        public string Username { get; set; } = null!;
        [DisplayName("First name")]
        public string FirstName { get; set; } = null!;
        [DisplayName("Last name")]
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        [DisplayName("Is confirmed")]
        public bool IsConfirmed { get; set; }
        [DisplayName("Country of residence")]
        public string CountryOfResidence { get; set; } = null!;
    }
}
