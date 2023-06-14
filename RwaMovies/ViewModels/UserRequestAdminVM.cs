using System.ComponentModel.DataAnnotations;
using RwaMovies.SharedModels.Auth;

namespace RwaMovies.ViewModels
{
    public class UserRequestAdminVM
    {
        public UserRequest UserRequest { get; set; } = null!;
        [Display(Name = "User confirmed")]
        public bool IsConfirmed { get; set; }
    }
}
