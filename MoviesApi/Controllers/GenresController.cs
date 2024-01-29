namespace MoviesApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGenresAsync()
        {
            var genres = await _genresService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenreAsync(CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _genresService.Create(genre);

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenreAsync(byte id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _genresService.GetById(id);

            if(genre is null)
                return NotFound($"No genre was found with id: {id}");

            genre.Name = dto.Name;

            _genresService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenreAsync(byte id)
        {
            var genre = await _genresService.GetById(id);

            if (genre is null)
                return NotFound($"No genre was found with id: {id}");

            _genresService.Delete(genre);
            
            return Ok(genre);
        }
    }
}
