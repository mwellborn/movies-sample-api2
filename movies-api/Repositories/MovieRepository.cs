using movies_api.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace movies_api.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieContext _context;

        public MovieRepository(MovieContext context)
        {
            _context = context;
        }

        public Task<List<Movie>> List()
        {
            return _context.Movies
                .ToListAsync();
        }

        public Task<Movie> GetById(int id)
        {
            return _context.Movies
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public Task Add(Movie movie)
        {
            _context.Movies.Add(movie);
            return _context.SaveChangesAsync();
        }

        public Task Update(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        public Task Delete(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Deleted;
            return _context.SaveChangesAsync();
        }
    }
}