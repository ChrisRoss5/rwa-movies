namespace RwaMovies.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string Username { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string NewPassword1 { get; set; } = null!;
        public string NewPassword2 { get; set; } = null!;

    }
}
