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
        public int statusId { get; set; }
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        public List<int> selectedCategoryIds { get; set; }
        [BindProperty]
        public List<int> SelectedCategories { get; set; } = new List<int>();

        public Update(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            var client = _clientFactory.CreateClient();

            try{
                //Book getirme
                var bookResponse = await client.GetAsync($"https://localhost:5014/api/Book/{id}");
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
                        ($"https://localhost:5014/api/ReadingTracking/readingTracking/{id}");

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
                                    ($"https://localhost:5014/api/Review/review/{id}");

                if (!reviewResponse.IsSuccessStatusCode) return NotFound();
                if(reviewResponse.ReasonPhrase == "No Content")  ReviewDto = new ReviewDto();
                else
                {
                    var review = await reviewResponse.Content.ReadFromJsonAsync<Review>();
                    if(review == null) return NotFound();
                    ReviewDto = new ReviewDto
                    {
                        Id = review.Id,
                        Text = review.Text,
                        Title = review.Title,
                        Liked = review.Liked
                    };                  
                }

                // tüm durumler getirme
                var statusResponse = await client.GetStringAsync("https://localhost:5014/api/Status");
                var statuses = JsonConvert.DeserializeObject<List<StatusDto>>(statusResponse);
                
                var selectedStatusResponse = await client.GetStringAsync($"https://localhost:5014/books/{id}");
                var selectedStatusId = JsonConvert.DeserializeObject<StatusDto>(selectedStatusResponse);

                 // StatusOptions'ı doldurme
                StatusOptions = statuses?.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList() ?? new List<SelectListItem>();

                // tüm kategoriler getirme
                var categoryResponse = await client.GetStringAsync("https://localhost:5014/api/Category");
                var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryResponse);

                var selectedCategoryResponse = await client.GetStringAsync($"https://localhost:5014/api/Category/category/{id}");
                var selectedCategories = JsonConvert.DeserializeObject<List<CategoryDto>>(selectedCategoryResponse);

                SelectedCategories = selectedCategories.Select(c => c.Id).ToList();
                // CategoryOptions'ı doldurme
                CategoryOptions = categories?.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), 
                    Text = c.Name,
                    Selected = SelectedCategories.Contains(c.Id)
                }).ToList() ?? new List<SelectListItem>();

                // Mevcut durumu ayarlama
                statusId = selectedStatusId.Id;

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
        
            // bookDto'yu JSON'a serileştirme
            var bookJson =  JsonConvert.SerializeObject(BookDto);
            var content = new StringContent(bookJson, Encoding.UTF8, "application/json");

            var categoryids = string.Join("", SelectedCategories.Select(catId => $"&catId={catId}"));
            // İstek URI'sini oluşturme
            var requestUri = $"https://localhost:5014/api/Book/{id}?statusId={statusId}" + $"{categoryids}" +
                                $"&StartDate={ReadingTrackingDto.StartDate:MM-dd-yyyy}&EndDate={ReadingTrackingDto.EndDate:MM-dd-yyyy}" +
                                $"&Title={ReviewDto.Title}&Text={ReviewDto.Text}&Liked={ReviewDto.Liked}";
            try{// İstek gönderme
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