using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs;

public partial class TagDTO
{
    public int? Id { get; set; }
    [StringLength(50)]
    public string Name { get; set; } = null!;
}
