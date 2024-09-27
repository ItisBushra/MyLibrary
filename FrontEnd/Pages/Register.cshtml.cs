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

namespace FrontEnd.Pages
{
    public class Register : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        [BindProperty]
        public RegisterDto RegisterDto { get; set; }
        private readonly ILogger<Register> _logger;
        public Register(ILogger<Register> logger, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var client = _clientFactory.CreateClient();
            var registerJson = JsonConvert.SerializeObject(RegisterDto);
            var content = new StringContent(registerJson, Encoding.UTF8, "application/json");
            try
            {
                var registerUri = await client.PostAsync
                                    ("https://localhost:5014/api/Account/register", content);
                if (registerUri.IsSuccessStatusCode)  return RedirectToPage("Index");
                else{
                    var errorResponse = await registerUri.Content.ReadAsStringAsync();
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