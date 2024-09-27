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
using PersonalizedLibraryAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FrontEnd.Pages
{
    public class Create : SharedBasePage
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserManager<AppUser> _userManager;
        [BindProperty]
        public BookDto BookDto { get; set; }
        [BindProperty]
        public ReviewDto ReviewDto { get; set; }
        [BindProperty] 
        public ReadingTrackingDto ReadingTrackingDto { get; set; }
        [BindProperty]
        public int catId { get; set; }
        [BindProperty]
        public int statusId { get; set; }
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        [BindProperty]
        public List<int> SelectedCategories { get; set; } = new List<int>();
        public bool IsAuthenticated { get; set; }
        [BindProperty]
        public string UserId { get; set; }

        public Create(IHttpClientFactory clientFactory, UserManager<AppUser> userManager)
        : base(userManager)
        {
            _clientFactory = clientFactory;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        { 
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Cookies.Delete("AuthToken");
                IsAuthenticated = false;
                return Page();
            }
            else
            {
                UserId = await GetUserIdFromTokenAsync(token);
                // API'den durumları getirin
                var client = _clientFactory.CreateClient();
                var response1 = await client.GetStringAsync("https://localhost:5014/api/Status");
                var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(response1);

                //kategorileri getirme
                var response2 = await client.GetStringAsync("https://localhost:5014/api/Category");
                var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(response2);

                // StatusOptions'ı Doldur
                StatusOptions = statuses.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(), 
                    Text = s.Name
                }).ToList();

                // CategoryOptions'ı Doldur 
                CategoryOptions = categories.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid){
                await OnGetAsync();
                return Page();
            }
            var token = Request.Cookies["AuthToken"];
            UserId = await GetUserIdFromTokenAsync(token);
            var client = _clientFactory.CreateClient();
            
            // bookDto'yu JSON'a serileştirme
            var bookJson =  JsonConvert.SerializeObject(BookDto);
            var content = new StringContent(bookJson, Encoding.UTF8, "application/json");
                
            var categoryids = string.Join("", SelectedCategories.Select(catId => $"&catId={catId}"));

            // İstek URI'sini oluşturun
            var requestUri = $"https://localhost:5014/api/Book?statusId={statusId}"+$"{categoryids}"+
            $"&StartDate={ReadingTrackingDto.StartDate:MM-dd-yyyy}&EndDate={ReadingTrackingDto.EndDate
            :MM-dd-yyyy}"+$"&Title={ReviewDto.Title}&Text={ReviewDto.Text}&Liked={ReviewDto.Liked}&userId={UserId}";
            try{
                // İstek gönderin
                var response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)  return RedirectToPage("Index");
                else{
                    var errorResponse = await response.Content.ReadAsStringAsync();
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