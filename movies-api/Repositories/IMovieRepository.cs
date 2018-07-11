using movies_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace movies_api.Repositories
{
    public interface IMovieRepository
    {
        Task<List<Movie>> List();
        Task<Movie> GetById(int id);
        Task Add(Movie movie);
        Task Update(Movie movie);
        Task Delete(Movie movie);
    }
}