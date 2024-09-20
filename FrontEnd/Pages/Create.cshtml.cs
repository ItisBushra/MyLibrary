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

namespace FrontEnd.Pages
{
    public class Create : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
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
        public Create(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // API'den durumları getirin
            var client = _clientFactory.CreateClient();
            var response1 = await client.GetStringAsync("http://localhost:5014/api/Status");
            var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(response1);

            //kategorileri getirme
            var response2 = await client.GetStringAsync("http://localhost:5014/api/Category");
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid){
                await OnGetAsync();
                return Page();
            }

            var client = _clientFactory.CreateClient();
            
                // Gerekli tüm verileri içeren yeni bir BookDto oluşturun
                var bookDto = new BookDto{
                    Name = BookDto.Name,
                    WritersName = BookDto.WritersName
                };
                // bookDto'yu JSON'a serileştirme
                var bookJson = JsonConvert.SerializeObject(bookDto);
                var content = new StringContent(bookJson, Encoding.UTF8, "application/json");
                // İstek URI'sini oluşturun
                var requestUri = $"http://localhost:5014/api/Book?statusId={statusId}&catId={catId}" +
                                $"&StartDate={ReadingTrackingDto.StartDate:MM-dd-yyyy}&EndDate={ReadingTrackingDto.EndDate:MM-dd-yyyy}" +
                                $"&Title={ReviewDto.Title}&Text={ReviewDto.Text}&Liked={ReviewDto.Liked}";
                try{
                    // İstek gönderin
                    var response = await client.PostAsync(requestUri, content);

                    if (response.IsSuccessStatusCode)  return RedirectToPage("Index");
                    else
                    {
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