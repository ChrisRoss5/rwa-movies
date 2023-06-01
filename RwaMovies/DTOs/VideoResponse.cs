using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs;

public partial class VideoResponse
{
    public int Id { get; set; }

	[Display(Name = "Created at")]
	public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public GenreDTO Genre { get; set; } = null!;

	[Display(Name = "Total seconds")]
	public int TotalSeconds { get; set; }

	[Display(Name = "Streaming URL")]
	public string? StreamingUrl { get; set; }

    public ImageDTO? Image { get; set; }

    public List<TagDTO> Tags { get; set; } = null!;
}
