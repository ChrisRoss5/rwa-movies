using System.ComponentModel.DataAnnotations;
using RwaMovies.Models.Shared.Auth;

namespace RwaMovies.Models.Views
{
    public class UserRequestAdminVM
    {
        public UserRequest UserRequest { get; set; } = null!;
        [Display(Name = "User confirmed")]
        public bool IsConfirmed { get; set; }
    }
}
