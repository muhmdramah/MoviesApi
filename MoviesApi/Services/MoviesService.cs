namespace MoviesApi.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext _context;

        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            var movies = await _context.Movies.
                Where(movie => movie.GenreId == genreId || genreId == 0).
                OrderByDescending(movie => movie.Rate).
                Include(movie => movie.Genre).
                ToListAsync();

            return movies;
        }

        public async Task<Movie> GetById(int id)
        {
            var movie = await _context.Movies.
                OrderByDescending(movie => movie.Rate).
                Include(movie => movie.Genre).
                SingleOrDefaultAsync(movie => movie.Id == id);

            return movie;
        }

        public async Task<Movie> Create(Movie movie)
        {
            await _context.AddAsync(movie);
            _context.SaveChanges();

            return movie;
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();

            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();

            return movie;
        }
    }
}
