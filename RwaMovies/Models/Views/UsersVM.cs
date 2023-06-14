using Microsoft.AspNetCore.Mvc.Rendering;
using RwaMovies.Models.Shared.Auth;

namespace RwaMovies.Models.Views
{
    public class UsersVM
    {
        public List<UserResponse> Users { get; set; } = null!;
        public SelectList Countries { get; set; } = null!;
        public string? FirstNameFilter { get; set; }
        public string? LastNameFilter { get; set; }
        public string? UsernameFilter { get; set; }
        public string? CountryOfResidenceFilter { get; set; }
    }
}
