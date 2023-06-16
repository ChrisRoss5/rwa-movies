using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared
{
    public class VideoResponse
    {
        public int Id { get; set; }
        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        [DisplayFormat(NullDisplayText = "—")]
        public string? Description { get; set; }
        public GenreDTO Genre { get; set; } = null!;
        [Display(Name = "Total seconds")]
        public int TotalSeconds { get; set; }
        [Display(Name = "Streaming URL"), DisplayFormat(NullDisplayText = "—")]
        public string? StreamingUrl { get; set; }
        public int? ImageId { get; set; }
        public List<TagDTO> Tags { get; set; } = null!;
    }
}
