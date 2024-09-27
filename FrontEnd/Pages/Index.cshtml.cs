using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.DTOs.Account;
using PersonalizedLibraryAPI.DTOs;
namespace FrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory  _clientFactory;
    [BindProperty]
    public LoginDto LoginDto { get; set; }
    public List<BookDetailsDto> Books { get; set; } = new List<BookDetailsDto>();
    public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
    public bool IsAuthenticated { get; set; }
    public IndexModel(IHttpClientFactory  clientFactory,
                        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = _clientFactory.CreateClient();
        try{
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Cookies.Delete("AuthToken");
                IsAuthenticated = false;
            }
            else
            {
                IsAuthenticated = true;
                TempData["SuccessMessage"] = "Girişiniz başarılı oldu!";

                var userEmail = GetEmailFromToken(token);
                
                var response = await client.GetAsync($"https://localhost:5014/api/Book/GetAll/{userEmail}");
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
            }
            return Page();
        }
        catch (Exception)
        {
            return StatusCode(500, "İstek işlenirken bir hata oluştu.");
        } 

    }
    private string GetEmailFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.First(claim => claim.Type == "email").Value;
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
            
            await OnGetAsync();
            return new JsonResult(new { success = true }); // Return success message
        }
        catch (Exception ex)
        {
            return StatusCode(500, "istek başarısız oldu: " + ex.Message);
        }
    }

    public async Task<IActionResult> OnPostLogout()
    {
        var client = _clientFactory.CreateClient();
        var token = Request.Cookies["AuthToken"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["AuthToken"]);
        try
        {
            var logoutUri = await client.PostAsync
                           ("https://localhost:5014/api/Account/logout", null);
            if (logoutUri.IsSuccessStatusCode)
            {
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("Index");
            } 
            else{
                var errorResponse = await logoutUri.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Hata: {errorResponse}");
            }
        }
        catch (HttpRequestException ex){
                ModelState.AddModelError("", $"HTTP isteğin Hatası: {ex.Message}");
        }
        return Page();
    }
}