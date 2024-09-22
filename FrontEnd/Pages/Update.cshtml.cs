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
    public class Update : PageModel
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

        public Update(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var client = _clientFactory.CreateClient();

            try{
                //Book getirme
                var bookResponse = await client.GetAsync($"http://localhost:5014/api/Book/{id}");
                if (!bookResponse.IsSuccessStatusCode) return NotFound();
                    
                var book = await bookResponse.Content.ReadFromJsonAsync<Book>();
                if (book == null) return NotFound();

                // BookDto doldur
                BookDto = new BookDto
                {
                    Id = book.Id,
                    Name = book.Name,
                    WritersName = book.WritersName
                };

                // ReadingTracking getirme
                var readingTrackingResponse = await client.GetAsync
                        ($"http://localhost:5014/api/ReadingTracking/readingTracking/{id}");

                if (!readingTrackingResponse.IsSuccessStatusCode) return NotFound();
                if(readingTrackingResponse.ReasonPhrase == "No Content") 
                    ReadingTrackingDto = new ReadingTrackingDto();
                else
                {
                    var readingTracking = await readingTrackingResponse.Content.ReadFromJsonAsync<ReadingTracking>();
                    if(readingTracking == null) return NotFound();

                    ReadingTrackingDto = new ReadingTrackingDto
                    {
                        Id = readingTracking.Id,
                        StartDate = readingTracking.StartDate,
                        EndDate = readingTracking.EndDate
                    };
                
                }

                //Review getirme
                var reviewResponse = await client.GetAsync
                                    ($"http://localhost:5014/api/Review/review/{id}");

                if (!reviewResponse.IsSuccessStatusCode) return NotFound();
                if(reviewResponse.ReasonPhrase == "No Content")
                    ReviewDto = new ReviewDto();
                else
                {
                    var review = await reviewResponse.Content.ReadFromJsonAsync<Review>();

                    if(review == null) return NotFound();

                    ReviewDto = new ReviewDto
                    {
                        Id = review.Id,
                        Text = review.Text,
                        Title = review.Title,
                        Liked = review.Liked,
                    };                  
                }

                // tüm durumler getirme
                var statusResponse = await client.GetStringAsync("http://localhost:5014/api/Status");
                var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(statusResponse);

                // tüm kategoriler getirme
                var categoryResponse = await client.GetStringAsync("http://localhost:5014/api/Category");
                var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryResponse);

                // StatusOptions'ı doldur
                StatusOptions = statuses?.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = s.Id == book.StatusId
                }).ToList() ?? new List<SelectListItem>();

                // CategoryOptions'ı doldur
                CategoryOptions = categories?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = book.BookCategories?
                                    .Any(bc => bc.CategoryId == c.Id) ?? false
                }).ToList() ?? new List<SelectListItem>();

                // Mevcut durumu ve kategoriyi ayarlama
                statusId = book.StatusId;
                catId = book.BookCategories?
                            .FirstOrDefault()?.CategoryId ?? 0;

                return Page();
            }
            catch (Exception ex){// İstisna durumunu kaydet
                   return StatusCode(500, "İstek işlenirken bir hata oluştu.");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid || id == null){
                await OnGetAsync(id);
                return Page();
            }
            var client = _clientFactory.CreateClient();
            
            // Gerekli tüm verileri içeren yeni bir BookDto oluşturun
            var bookDto = new BookDto{
                Id = id,
                Name = BookDto.Name,
                WritersName = BookDto.WritersName
                };
            // bookDto'yu JSON'a serileştirme
            var bookJson = JsonConvert.SerializeObject(bookDto);
            var content = new StringContent(bookJson, Encoding.UTF8, "application/json");
            // İstek URI'sini oluşturun
            var requestUri = $"http://localhost:5014/api/Book/{id}?statusId={statusId}&catId={catId}" +
                                $"&StartDate={ReadingTrackingDto.StartDate:MM-dd-yyyy}&EndDate={ReadingTrackingDto.EndDate:MM-dd-yyyy}" +
                                $"&Title={ReviewDto.Title}&Text={ReviewDto.Text}&Liked={ReviewDto.Liked}";
            try{// İstek gönderin
                var response = await client.PutAsync(requestUri, content);

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
