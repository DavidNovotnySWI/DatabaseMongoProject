using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;

namespace ClientApp.Models
{
    public class BookCreateViewModel
    {
        public Book Book { get; set; }
        public SelectList Authors { get; set; }
    }
}
