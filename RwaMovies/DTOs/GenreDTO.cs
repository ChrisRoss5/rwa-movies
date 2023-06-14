using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs
{
    public class GenreDTO
    {
        public int? Id { get; set; }
        [StringLength(256)]
        public string Name { get; set; } = null!;
        [StringLength(1024)]
        public string? Description { get; set; }
    }
}