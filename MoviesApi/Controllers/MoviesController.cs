namespace MoviesApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;

        private List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        [HttpGet]    
        public async Task<IActionResult> GetAllMoviesAsync()
        {
            var allMovies = await _moviesService.GetAll();

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(allMovies);

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie is null)
                return NotFound($"Movie with id {id} is not found!");
              
            var dto = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(dto);
        }

        [HttpGet("GetMovieByGenreId")]
        public async Task<IActionResult> GetMovieByGenreIdAsync(byte genreId)
        {
            var movie = await _moviesService.GetAll(genreId);

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movie);

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovieAsync([FromForm] CreateMovieDto dto)
        {
            var extension = Path.GetExtension(dto.Poster.FileName).ToLower();

            if (!_allowedExtenstions.Contains(extension))
                return BadRequest("Only .png and .jpg images are allowed!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genre ID!"); 

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();

            _moviesService.Create(movie);

            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovieAsync(int id, [FromBody] UpdateMovieDto dto)
        {
            var movie = await _moviesService.GetById(id);

            if (movie is null)
                return NotFound($"No movie was found with id {id}");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            if(dto.Poster != null)
            {
                var extension = Path.GetExtension(dto.Poster.FileName).ToLower();

                if (!_allowedExtenstions.Contains(extension))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");

                using var dataStream = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.ProductionYear = dto.ProductionYear;
            movie.Storeline = dto.Storeline;
            movie.Rate = dto.Rate;

            _moviesService.Update(movie);

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if(movie is null)
                return NotFound($"No movie was found with id {id}");

            _moviesService.Delete(movie);

            return Ok(movie);
        }
    }
}