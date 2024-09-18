using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PersonalizedLibraryAPI.Models;
namespace FrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    public IEnumerable<Book> Books { get; set; }

    public IndexModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("http://localhost:5014/api/Book/GetAll");
            Books = JsonConvert.DeserializeObject<List<Book>>(response)?? new List<Book>(); 
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Bir hata olmuştur: {ex.Message}");
            Books = new List<Book>(); 
        }
    }
}
