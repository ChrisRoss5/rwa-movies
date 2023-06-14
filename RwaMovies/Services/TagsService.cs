using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Exceptions;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared;

namespace RwaMovies.Services
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDTO>> GetTags();
        Task<TagDTO> GetTag(int id);
        Task PutTag(int id, TagDTO tagDTO);
        Task<int> PostTag(TagDTO tagDTO);
        Task DeleteTag(int id);
    }

    public class TagsService : ITagsService
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public TagsService(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagDTO>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return _mapper.Map<List<TagDTO>>(tags);
        }

        public async Task<TagDTO> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                throw new NotFoundException();
            return _mapper.Map<TagDTO>(tag);
        }

        public async Task PutTag(int id, TagDTO tagDTO)
        {
            try
            {
                var tag = _mapper.Map<Tag>(tagDTO);
                _context.Entry(tag).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!TagExists(id))
                    throw new NotFoundException();
                throw;
            }
        }

        public async Task<int> PostTag(TagDTO tagDTO)
        {
            var tag = _mapper.Map<Tag>(tagDTO);
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag.Id;
        }

        public async Task DeleteTag(int id)
        {
            var tag = await _context.Tags.Include(t => t.VideoTags).FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
                throw new NotFoundException();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        private bool TagExists(int id)
        {
            return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
