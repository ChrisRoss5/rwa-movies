using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared.Auth
{
    public class UserRequest
    {
        public int? Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;
        [DisplayName("Password"), DataType(DataType.Password)]
        public string Password1 { get; set; } = null!;
        [DisplayName("Confirm password"), DataType(DataType.Password)]
        [Compare("Password1", ErrorMessage = "Passwords do not match.")]
        public string Password2 { get; set; } = null!;
        [DisplayName("First name"), StringLength(256)]
        public string FirstName { get; set; } = null!;
        [DisplayName("Last name"), StringLength(256)]
        public string LastName { get; set; } = null!;
        [EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;
        [Phone, StringLength(256)]
        public string? Phone { get; set; }
        [DisplayName("Is confirmed")]
        public bool IsConfirmed { get; set; }
        [DisplayName("Country of residence")]
        public int CountryOfResidenceId { get; set; }
    }
}
