using System.ComponentModel;

namespace RwaMovies.DTOs.Auth
{
    public class NewPasswordRequest
    {
        public AuthRequest AuthRequest { get; set; } = null!;
        [DisplayName("New password")]
        public string NewPassword1 { get; set; } = null!;
        [DisplayName("Confirm new password")]
        public string NewPassword2 { get; set; } = null!;

    }
}
