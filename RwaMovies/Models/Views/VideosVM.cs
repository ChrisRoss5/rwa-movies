using Microsoft.AspNetCore.Mvc.Rendering;
using RwaMovies.Services;

namespace RwaMovies.Models.Views
{
    public class VideosVM
    {
        public SearchResult SearchResult { get; set; } = null!;
        public SelectList Genres { get; set; } = null!;
        public string? GenreFilter { get; set; }
        public string? NameFilter { get; set; }
    }
}
