

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Book
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Publish year is required.")]
        [Range(1800, 2100, ErrorMessage = "Publish year must be between 1800 and 2100.")]
        public int? PublisheDate { get; set; }
        [Required(ErrorMessage = "The Genre field is required.")]
        public string? Genre { get; set; } = null;
        //[Required(ErrorMessage = "The Description field is required.")]
        [StringLength(500, ErrorMessage = "The Description must be less than 500 characters.")]
        public string? Description { get; set; } = null;
        public string? Picture { get; set; } = null;
        [Required(ErrorMessage = "The Pages field is required.")]
        [Range(1, 10000, ErrorMessage = "Pages must be between 1 and 10 000.")]
        public int Pages { get; set; } = 0;
        [Required(ErrorMessage = "The Price field is required.")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1 000 000 Kč.")]
        public int Price { get; set; } = 0;
        public string AuthorId { get; set; } = string.Empty; // Reference na autora
    }
}
