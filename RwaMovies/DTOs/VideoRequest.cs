using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs;

public partial class VideoRequest
{
	public int? Id { get; set; }
	[StringLength(256)]
	public string Name { get; set; } = null!;
	[StringLength(1024)]
	public string? Description { get; set; }
	[Display(Name = "Genre")]
	public int GenreId { get; set; }
	[Display(Name = "Total seconds"), Range(1, int.MaxValue)]
	public int TotalSeconds { get; set; }
	[Display(Name = "Streaming URL"), Url, StringLength(256)]
	public string? StreamingUrl { get; set; }
	public int? ImageId { get; set; }
	[Display(Name = "Tags")]
	public List<int>? TagIds { get; set; }
}
