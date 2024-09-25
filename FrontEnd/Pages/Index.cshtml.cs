using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.DTOs;
namespace FrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory  _clientFactory;
    public List<BookDetailsDto> Books { get; set; } = new List<BookDetailsDto>();
    public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
    public IndexModel(IHttpClientFactory  clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = _clientFactory.CreateClient();
        try{
            var response = await client.GetAsync("https://localhost:5014/api/Book/GetAll");
            if(!response.IsSuccessStatusCode) return NotFound();
            var booksJson = await response.Content.ReadAsStringAsync();

            Books = JsonConvert.DeserializeObject<List<BookDetailsDto>>(booksJson) ?? new List<BookDetailsDto>();
            
            var statusResponse = await client.GetStringAsync("https://localhost:5014/api/Status");
            var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(statusResponse);
                
            StatusOptions = statuses?.Select(s => new SelectListItem{
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList() ?? new List<SelectListItem>();

            var categoryResponse = await client.GetStringAsync("https://localhost:5014/api/Category");
            var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryResponse);
                
            CategoryOptions = categories?.Select(c => new SelectListItem{
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList() ?? new List<SelectListItem>();

            return Page();
        }
        catch (Exception)
        {
            return StatusCode(500, "İstek işlenirken bir hata oluştu.");
        } 
    }

    public async Task<IActionResult> OnDeleteAsync(int id)
    {
        if (!ModelState.IsValid || id == null){
            await OnGetAsync();
            return Page();
        }
        var client = _clientFactory.CreateClient();
        try
        {
            var response = await client.DeleteAsync($"https://localhost:5014/api/Book/{id}");
            
            if (!response.IsSuccessStatusCode)return NotFound();
            return new JsonResult(new { success = true });
            
            
            await OnGetAsync();
            return new JsonResult(new { success = true }); // Return success message
        }
        catch (Exception ex)
        {
            return StatusCode(500, "istek başarısız oldu: " + ex.Message);
        }
    }
}
