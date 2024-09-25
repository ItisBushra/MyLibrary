using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalizedLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PersonalizedLibraryAPI.DTOs.Account;

namespace FrontEnd.Pages.Shared
{
    public class Login : PageModel
    {
        
        private readonly IHttpClientFactory _clientFactory;
        [BindProperty]
        public LoginDto LoginDto { get; set; }
        private readonly ILogger<Login> _logger;

        public Login(ILogger<Login> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var client = _clientFactory.CreateClient();
            var loginJson = JsonConvert.SerializeObject(LoginDto);
            var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            try
            {
                var loginUri = await client.PostAsync
                                    ("https://localhost:5014/api/Account/login", content);
                if (loginUri.IsSuccessStatusCode)  return RedirectToPage("Index");
                else{
                    var errorResponse = await loginUri.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Hata: {errorResponse}");
                }
            }
            catch (HttpRequestException ex){
                ModelState.AddModelError("", $"HTTP isteğin Hatası: {ex.Message}");
            }
            return Page();
        }
    }
}