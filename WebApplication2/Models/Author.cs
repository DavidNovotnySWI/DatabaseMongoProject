
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Author
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int? Age { get; set; }
        //[Url(ErrorMessage = "Please enter a valid URL.")]
        //[Required(ErrorMessage = "Image URL is required.")]
        public string? Image { get; set; } = null;
        public List<Book> Books { get; set; } = new(); // Book colection of authors

    }
}
