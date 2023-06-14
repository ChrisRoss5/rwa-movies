using System.ComponentModel.DataAnnotations;

namespace RwaMovies.SharedModels
{
    public class CountryDTO
    {
        public int? Id { get; set; }
        [StringLength(2, MinimumLength = 2, ErrorMessage = "This field must be 2 characters")]
        public string Code { get; set; } = null!;
        [StringLength(256)]
        public string Name { get; set; } = null!;
    }
}
