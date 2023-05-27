using Microsoft.AspNetCore.Mvc.Rendering;

namespace RwaMovies.DTOs
{
    public class VideoGenreViewModel
    {
        public List<VideoResponse> Videos { get; set; } = null!;
        public SelectList Genres { get; set; } = null!;
        public string? GenreFilter { get; set; }
        public string? NameFilter { get; set; }
    }
}
