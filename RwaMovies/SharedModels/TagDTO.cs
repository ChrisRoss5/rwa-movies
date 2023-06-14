using System.ComponentModel.DataAnnotations;

namespace RwaMovies.SharedModels
{
    public class TagDTO
    {
        public int? Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
