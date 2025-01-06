using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorService authorService;
        private readonly BookService BookService;

        public AuthorController(AuthorService authorService) =>
            this.authorService = authorService;

        [HttpGet]
        public async Task<List<Author>> Get() =>
            await authorService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Author>> Get(string id)
        {
            var author = await authorService.GetAsync(id);

            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var author = await authorService.GetAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            await authorService.RemoveAsync(id);

            return NoContent();
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, Author updatedAuthor)
        {
            var author = await authorService.GetAsync(id);

            if (author == null)
            {
                return NotFound();
            }
            updatedAuthor.Id = author.Id;

            await authorService.UpdateAsync(id, updatedAuthor);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            await authorService.CreateAsync(author);

            return CreatedAtAction(nameof(Get), new { id = author.Id }, author);

        }
        [HttpPost("bulk")]
        public async Task<ActionResult> AddMultiple([FromBody] List<Author> authors)
        {
            if (authors == null || authors.Count == 0)
            {
                return BadRequest("The authors list cannot be empty.");
            }

            await authorService.AddMultipleAsync(authors);
            return Ok($"{authors.Count} authors added successfully.");
        }

        [HttpGet("filter-by-lastname/{lastName}")]
        public async Task<ActionResult<List<Author>>> FilterByLastName([FromRoute] string lastName)
        {
            var authors = await authorService.FilterByLastNameAsync(lastName);

            if (authors.Count == 0)
            {
                return NotFound($"No authors found with last name '{lastName}'.");
            }

            return Ok(authors);
        }

        [HttpGet("younger-than")]
        public async Task<ActionResult<List<Author>>> GetYoungerThan([FromQuery] int age)
        {
            if (age <= 0)
            {
                return BadRequest("Age must be greater than 0.");
            }

            var authors = await authorService.GetYoungerThanAsync(age);

            if (authors.Count == 0)
            {
                return NotFound($"No authors found younger than {age}.");
            }

            return Ok(authors);
        }

        [HttpGet("older-than")]
        public async Task<ActionResult<List<Author>>> GetOlderThan([FromQuery] int age)
        {
            if (age <= 0)
            {
                return BadRequest("Age must be greater than 0.");
            }
            var authors = await authorService.GetOlderThanAsync(age);
            if (authors.Count == 0)
            {
                return NotFound($"No authors found older than {age}.");
            }
            return Ok(authors);
        }


    }

}
