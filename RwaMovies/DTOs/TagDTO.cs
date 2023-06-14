using System.ComponentModel.DataAnnotations;

namespace RwaMovies.DTOs
{
    public class TagDTO
    {
        public int? Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
