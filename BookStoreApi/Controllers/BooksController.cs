using AutoMapper;
using BookStoreApi.Data.Repository.Implementations;
using BookStoreApi.Data.Repository.Interfaces;
using BookStoreApi.Models;
using BookStoreApi.Models.DTO.Author;
using BookStoreApi.Models.DTO.Book;
using BookStoreApi.Models.DTO.Genre;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IRepository<Author> authorRepository, IRepository<Genre> genreRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _bookRepository.GetAllAsync();
            var booksDto = books.Select(book => _mapper.Map<BookDto>(book));
            return Ok(booksDto);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookDto>(book));
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody] BookDto bookDto)
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
        public async Task<IActionResult> PutBook(int id, [FromBody] BookDto bookDto)
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
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? genre)
        {
            var books = await _bookRepository.SearchBooksAsync(title, author, genre);

            if (books == null || !books.Any())
            {
                return NotFound("No books found matching the criteria.");
            }

            
            var bookDtos = books.Select(book => _mapper.Map<BookDto>(book));

            return Ok(bookDtos);
        }
    }
}
