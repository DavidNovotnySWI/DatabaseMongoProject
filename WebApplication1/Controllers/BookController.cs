using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookService bookService;

        public BookController(BookService bookService) =>
            this.bookService = bookService;

        [HttpGet]
        public async Task<List<Book>> Get() =>
            await bookService.GetAsync();

        [HttpGet("by-id")]
        public async Task<Book> Get(string id) =>
            await bookService.GetAsync(id);

        [HttpGet("by-title")]
        public async Task<Book> GetBook(string title) =>
            await bookService.GetBookAsync(title);

        [HttpGet("by-genre")]
        public async Task<Book> GetBookByGenre(string genre) =>
            await bookService.GetBookByGenreAsync(genre);

        [HttpGet("filter-by-title")]
        public async Task<List<Book>> FilterByTitle(string title) =>
            await bookService.FilterByTitleAsync(title);

        [HttpGet("by-author")]
        public async Task<Book> GetBookByAuthor(string authorId) =>
            await bookService.GetBookByAuthorAsync(authorId);

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string query, [FromQuery] string? category = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var books = await bookService.SearchBooksAsync(query, category);
            return Ok(books);
        }


        [HttpPost]
        public async Task Create(Book newBook) =>
            await bookService.CreateAsync(newBook);

        [HttpPut("{id:length(24)}")]
        public async Task Update(string id, Book updatedBook) =>
            await bookService.UpdateAsync(id, updatedBook);

        [HttpDelete("{id:length(24)}")]
        public async Task Remove(string id) =>
            await bookService.RemoveAsync(id);

        [HttpPost("bulk")]
        public async Task AddMultiple(List<Book> books) =>
            await bookService.AddMultipleAsync(books);


        [HttpGet("book-with-student")]
        public async Task<IActionResult> GetStudentWithBooks(string id)
        {
            var student = await bookService.GetStudentByBookIdAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(student);
        }
    }
}
