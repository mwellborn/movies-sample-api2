using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using movies_api.Models;
using movies_api.Repositories;

namespace movies_api.Controllers
{
    public class MoviesController : ApiController
    {
        //private MovieContext db = new MovieContext();
        private IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        // GET: api/Movie
        public async Task<IHttpActionResult> GetMovies()
        {
            var movies = await _movieRepository.List();

            return Ok(movies);
        }

        // GET: api/Movie/5
        [ResponseType(typeof(Movie))]
        public async Task<IHttpActionResult> GetMovie(int id)
        {
            var movie = await _movieRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // PUT: api/Movie/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMovie(int id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movie.ID)
            {
                return BadRequest();
            }

            try
            {
                await _movieRepository.Update(movie);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Movie
        [ResponseType(typeof(Movie))]
        public async Task<IHttpActionResult> PostMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _movieRepository.Add(movie);

            return CreatedAtRoute("MovieApi", new { id = movie.ID }, movie);
        }

        // DELETE: api/Movie/5
        [ResponseType(typeof(Movie))]
        public async Task<IHttpActionResult> DeleteMovie(int id)
        {
            Movie movie = await _movieRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }
            
            await _movieRepository.Delete(movie);

            return Ok(movie);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private bool MovieExists(int id)
        {
            if (_movieRepository.GetById(id).Result.ID == id)
            {
                return true;
            }
            return false;
        }
    }
}