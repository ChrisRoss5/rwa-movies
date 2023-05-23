﻿using System;
using System.Collections.Generic;

namespace RwaMovies.DTOs;

public partial class GenreDTO
{
    public int? Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}