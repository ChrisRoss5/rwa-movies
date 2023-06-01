using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;

namespace RwaMovies.Services
{
	public interface IGenresService
    {
        Task<IEnumerable<GenreDTO>> GetGenres();
        Task<GenreDTO> GetGenre(int id);
        Task PutGenre(int id, GenreDTO genreDTO);
        Task<int> PostGenre(GenreDTO genreDTO);
        Task DeleteGenre(int id);
    }

    public class GenresService : IGenresService
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public GenresService(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreDTO>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        public async Task<GenreDTO> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                throw new NotFoundException();
            return _mapper.Map<GenreDTO>(genre);
        }

        public async Task PutGenre(int id, GenreDTO genreDTO)
        {
            try
            {
                var genre = _mapper.Map<Genre>(genreDTO);
                _context.Entry(genre).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!GenreExists(id))
                    throw new NotFoundException();
                throw;
            }
        }

        public async Task<int> PostGenre(GenreDTO genreDTO)
        {
            var genre = _mapper.Map<Genre>(genreDTO);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                throw new NotFoundException();
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        private bool GenreExists(int id)
        {
            return (_context.Genres?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
