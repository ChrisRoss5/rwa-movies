namespace RwaMovies.DTOs.Auth
{
    public class ValidateEmailRequest
    {
        public string Username { get; set; } = null!;
        public string B64SecToken { get; set; } = null!;
    }
}
