namespace RwaMovies.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
