using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        public async Task<IActionResult> Index(string description, string category)
        {
            string url;

            // Pokud nevypln� ��dn� filtr, zobraz�me v�echny knihy
            if (string.IsNullOrEmpty(description) && string.IsNullOrEmpty(category))
            {
                url = "https://localhost:7222/api/Book";
            }
            else
            {
                // Jinak zavol�me search endpoint s filtry
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
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Book/book-with-student?id=" + Id).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<Student>(responseData);
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
        public async Task<IActionResult> Create(Student model)
        {
            try
            {

                string requestDataJson = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync("https://localhost:7222/" + $"api/Student/{model.Id}", content).Result;

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
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Student").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    var viewmodel = new BookCreateViewModel();
                    viewmodel.Book = new Book();
                    
                    var students = JsonConvert.DeserializeObject<List<Student>>(responseData);
                    viewmodel.Authors = new SelectList(students, "Id", "FirstName","LastName");



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

        public IActionResult Edit(string Id)
        {
            try
            {

                // Serializace objektu do form�tu JSON
                var requestDataJson = JsonConvert.SerializeObject(Id);
                var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.GetAsync("https://localhost:7222/" + "api/Student/" + Id).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<Student>(responseData);
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
        public async Task<IActionResult> Edit(Student student)
        {
            try
            {                  

                string requestDataJson = JsonConvert.SerializeObject(student);
                StringContent content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync("https://localhost:7222/" + $"api/Student/{student.Id}", content).Result;

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
            HttpResponseMessage response = await _httpClient.DeleteAsync("https://localhost:7222/" + "api/Student/" + Id);

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
