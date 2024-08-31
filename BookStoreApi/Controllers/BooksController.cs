using AutoMapper;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IMapper _mapper;

        public BooksController(IRepository<Book> bookRepository, IRepository<Author> authorRepository, IRepository<Genre> genreRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookRepository.GetAllAsync();
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] InputBookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if ((await _authorRepository.GetByIdAsync(bookDto.AuthorId)) == null || (await _genreRepository.GetByIdAsync(bookDto.GenreId)) == null)
            {
                return BadRequest("Invalid AuthorId or GenreId");
            }

            var book = _mapper.Map<Book>(bookDto);
            await _bookRepository.AddAsync(book);
            return CreatedAtAction("GetBook", new { id = book.BookId }, bookDto);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] InputBookDto bookDto)
        {
            if (id != bookDto.BookId)
            {
                return BadRequest("Book ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _bookRepository.UpdateAsync(_mapper.Map<Book>(bookDto));
            return NoContent();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            await _bookRepository.RemoveAsync(book);
            return NoContent();
        }
    }
}
