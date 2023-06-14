using Microsoft.AspNetCore.Mvc.Rendering;
using RwaMovies.SharedModels.Auth;

namespace RwaMovies.ViewModels
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
