using AutoMapper;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Genre;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IRepository<Genre> _genreRepository;
        private readonly IMapper _mapper;

        public GenresController(IRepository<Genre> genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            var genres = await _genreRepository.GetAllAsync();
            return Ok(genres);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre([FromBody] InputGenreDto genreDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.AddAsync(genre);
            return CreatedAtAction("GetGenre", new { id = genre.GenreId }, genre);
        }

        // PUT: api/Genres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, [FromBody] Genre genreDto)
        {
            if (id != genreDto.GenreId)
            {
                return BadRequest("Genre ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _genreRepository.UpdateAsync(_mapper.Map<Genre>(genreDto));
            return NoContent();
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            await _genreRepository.RemoveAsync(genre);
            return NoContent();
        }
    }
}
