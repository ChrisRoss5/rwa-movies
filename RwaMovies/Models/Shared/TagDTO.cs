using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared
{
    public class TagDTO
    {
        public int? Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
