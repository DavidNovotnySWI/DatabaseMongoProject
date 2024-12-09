
namespace WebApplication1.Models
{
    public class Student
    {
        public string? Id { get; set; }

        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public int Age { get; set; } = 0;
        public List<Book> Books { get; set; } = new(); // Book colection of authors

    }
}
