using System;
using System.Collections.Generic;

namespace RwaMovies.DTOs;

public partial class VideoResponse
{
    public int? Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public GenreDTO Genre { get; set; } = null!;

    public int TotalSeconds { get; set; }

    public string? StreamingUrl { get; set; }

    public ImageDTO? Image { get; set; }

    public List<TagDTO> Tags { get; set; } = null!;
}
