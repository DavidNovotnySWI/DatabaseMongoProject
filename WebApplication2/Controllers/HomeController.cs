using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WebApplication1.Models;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        Uri BaseAdress = new Uri("https://localhost:7222/");
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> IndexAsync()
        {
            HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Book").Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<Book>>(responseData);
                return View(books);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? description = null, string? category = null)
        {
            ViewData["description"] = description;
            ViewData["category"] = category;

            string url;

            if (string.IsNullOrEmpty(description) && string.IsNullOrEmpty(category))
            {
                url = "https://localhost:7222/api/Book";
            }
            else
            {
                url = $"https://localhost:7222/api/Book/search?query={Uri.EscapeDataString(description ?? "")}";
                if (!string.IsNullOrEmpty(category))
                {
                    url += $"&category={Uri.EscapeDataString(category)}";
                }
            }

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["errorMessage"] = "Failed to load books.";
                return View(new List<Book>());
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<Book>>(responseData);
            return View(books);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(string Id)
        {
            try
            {

                // Serializace objektu do form�tu JSON
                var requestDataJson = JsonConvert.SerializeObject(Id);
                var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Book/book-with-author?id=" + Id).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<Author>(responseData);
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Author model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                string requestDataJson = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync("https://localhost:7222/" + $"api/Author/{model.Id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Edit Success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "tereate the author.";
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddBookToAuthor(string id)
        {
            @ViewData["AuthorId"] = id;

            var viewmodel = new BookCreateViewModel();
            viewmodel.Authors = null;
            viewmodel.Book = new Book();
            viewmodel.Book.AuthorId = id;
            return View("BookCreate", viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> BookCreate()
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Author").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    var viewmodel = new BookCreateViewModel();
                    viewmodel.Book = new Book();

                    var authors = JsonConvert.DeserializeObject<List<Author>>(responseData);
                    var authorSelectList = authors
                       .Select(a => new
                       {
                           Id = a.Id,
                           FullName = $"{a.LastName} {a.FirstName}"
                       })
                       .ToList();
                    viewmodel.Authors = new SelectList(authorSelectList, "Id", "FullName");



                    return View(viewmodel);
                }
                else
                {
                    TempData["errorMessage"] = response.ReasonPhrase;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookCreate(BookCreateViewModel viewModel)
        {
            // duplicitni try pro zobrazeni nabidky autoru i po refreshy a pri validaci (nepodarilo se mi vyresit lepe)
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7222/api/Author");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var authors = JsonConvert.DeserializeObject<List<Author>>(responseData);
                    var authorSelectList = authors
                        .Select(a => new
                        {
                            Id = a.Id,
                            FullName = $"{a.LastName} {a.FirstName}"
                        })
                        .ToList();
                    viewModel.Authors = new SelectList(authorSelectList, "Id", "FullName");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to load authors.";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Error loading authors: {ex.Message}";
            }

            try
            {
                string requestDataJson = JsonConvert.SerializeObject(viewModel.Book);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync("https://localhost:7222/" + $"api/Book/{viewModel.Book.Id}", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Book Created.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        public IActionResult Edit(string Id)
        {
            try
            {

                // Serializace objektu do form�tu JSON
                var requestDataJson = JsonConvert.SerializeObject(Id);
                var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Author/" + Id).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<Author>(responseData);
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Author author)
        {
            try
            {

                string requestDataJson = JsonConvert.SerializeObject(author);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync("https://localhost:7222/" + $"api/Author/{author.Id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Edit Success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = response.ReasonPhrase;
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var requestDataJson = JsonConvert.SerializeObject(Id);
            var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.DeleteAsync("https://localhost:7222/" + "api/Author/" + Id);

            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Account Deleted.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = response.ReasonPhrase;
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Filter()
        {

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> BookEdit(string id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7222/api/Book/by-id?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var book = JsonConvert.DeserializeObject<Book>(responseData);
                    return View(book);
                }
                TempData["errorMessage"] = "Failed to load book details.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BookEdit(Book book)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors and the current book data
                return View(book);
            }

            try
            {

                string requestDataJson = JsonConvert.SerializeObject(book);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync("https://localhost:7222/" + $"api/Book/{book.Id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Edit Success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to update the book.";
                    return View(book);
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(string Id)
        {
            var requestDataJson = JsonConvert.SerializeObject(Id);
            var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.DeleteAsync("https://localhost:7222/" + "api/Book/" + Id);

            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Book Deleted.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = response.ReasonPhrase;
                return RedirectToAction("Index");
            }

        }
    }
}
