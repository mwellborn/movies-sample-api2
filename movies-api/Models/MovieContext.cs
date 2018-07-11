using System.Data.Entity;

namespace movies_api.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }
}