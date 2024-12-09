using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService studentService;

        public StudentController(StudentService studentService) =>
            this.studentService = studentService;

        [HttpGet]
        public async Task<List<Student>> Get() =>
            await studentService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Student>> Get(string id)
        {
            var student = await studentService.GetAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult> Delete(string id)
        {
            var student = await studentService.GetAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            await studentService.RemoveAsync(id);
            return NoContent();
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult> Update(string id, Student updatedStudent)
        {
            var student = await studentService.GetAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            updatedStudent.Id = student.Id;

            await studentService.UpdateAsync(id, updatedStudent);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Student student)
        {
            await studentService.CreateAsync(student);

            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);

        }
        [HttpPost("bulk")]
        public async Task<ActionResult> AddMultiple([FromBody] List<Student> students)
        {
            if (students == null || students.Count == 0)
            {
                return BadRequest("The students list cannot be empty.");
            }

            await studentService.AddMultipleAsync(students);
            return Ok($"{students.Count} students added successfully.");
        }

        [HttpGet("filter-by-lastname/{lastName}")]
        public async Task<ActionResult<List<Student>>> FilterByLastName([FromRoute] string lastName)
        {
            var students = await studentService.FilterByLastNameAsync(lastName);

            if (students.Count == 0)
            {
                return NotFound($"No students found with last name '{lastName}'.");
            }

            return Ok(students);
        }

        [HttpGet("younger-than")]
        public async Task<ActionResult<List<Student>>> GetYoungerThan([FromQuery] int age)
        {
            if (age <= 0)
            {
                return BadRequest("Age must be greater than 0.");
            }

            var students = await studentService.GetYoungerThanAsync(age);

            if (students.Count == 0)
            {
                return NotFound($"No students found younger than {age}.");
            }

            return Ok(students);
        }

        [HttpGet("older-than")]
        public async Task<ActionResult<List<Student>>> GetOlderThan([FromQuery] int age)
        {
            if (age <= 0)
            {
                return BadRequest("Age must be greater than 0.");
            }
            var students = await studentService.GetOlderThanAsync(age);
            if (students.Count == 0)
            {
                return NotFound($"No students found older than {age}.");
            }
            return Ok(students);
        }


        [HttpGet("students-with-book")]
        public async Task<IActionResult> GetStudentWithBooks(string id)
        {
            var student = await studentService.GetStudentWithBooksAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(student);
        }
    }

}
