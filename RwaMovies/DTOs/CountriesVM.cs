namespace RwaMovies.DTOs
{
    public class CountriesVM
    {
        public List<CountryDTO> Countries { get; set; } = null!;
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int PageCount { get; set; }
    }
}
