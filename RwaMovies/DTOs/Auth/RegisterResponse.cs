namespace RwaMovies.DTOs.Auth
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string SecurityToken { get; set; } = null!;
    }
}
