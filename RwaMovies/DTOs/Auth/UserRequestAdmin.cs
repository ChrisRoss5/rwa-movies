using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs.Auth
{
    public class UserRequestAdmin
    {
        public UserRequest UserRequest { get; set; } = null!;
        [Display(Name = "User confirmed")]
        public bool IsConfirmed { get; set; }
    }
}
