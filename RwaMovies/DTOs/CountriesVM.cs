using RwaMovies.Models;

namespace RwaMovies.DTOs
{
    public class CountriesVM
    {
        public List<Country> Countries { get; set; } = null!;
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int PageCount { get; set; }
    }
}
