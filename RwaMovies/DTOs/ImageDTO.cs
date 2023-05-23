using System;
using System.Collections.Generic;

namespace RwaMovies.DTOs;

public partial class ImageDTO
{
    public int? Id { get; set; }

    public string Content { get; set; } = null!;
}
