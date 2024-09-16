using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.Repository.IRepository;

namespace PersonalizedLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;

        private readonly IMapper _mapper;
        public BookController(IBookRepository bookRepository, IMapper mapper)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        public IActionResult GetBooks()
        {
            var books = _mapper.Map<List<BookDto>>(_bookRepository.GetBooks());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(books);
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        public IActionResult GetBook(int bookId)
        {
            if(!_bookRepository.BookExists(bookId))
                return NotFound();

            var book = _mapper.Map<BookDto>(_bookRepository.GetBook(bookId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(book);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateBook([FromQuery] int statusId, [FromQuery] int catId, 
                                        [FromBody] BookDto bookCreate, [FromQuery] ReadingTrackingDto?
                                         readingTracking, [FromQuery] ReviewDto? review)
        {
            if (bookCreate == null|| !ModelState.IsValid) 
                return BadRequest(ModelState);

            //kitap yeni olduğunu knotrol etmek
            var books = _bookRepository.GetBooks().Where(b=>b.Name == bookCreate.Name).FirstOrDefault();
            if (books != null)
            {
                ModelState.AddModelError("", "kitap mevcut");
                return StatusCode(422, ModelState);
            }

            var bookMap = _mapper.Map<Book>(bookCreate);
            var readingTrackingMap = _mapper.Map<ReadingTracking>(readingTracking);
            var reviewMap = _mapper.Map<Review>(review);

            if (!_bookRepository.CreateBook(catId, statusId, bookMap,
                         readingTrackingMap, reviewMap))
            {
                ModelState.AddModelError("", "bir şeyler ters gitti");
                return StatusCode(500, ModelState);
            }

            return Ok("başarıyla oluşturuldu");
        }

        [HttpPut("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateBook([FromQuery] int catId, [FromQuery] int statusId, int bookId, [FromBody] BookDto updateBook,[FromQuery] ReadingTrackingDto?
                                         readingTracking, [FromQuery] ReviewDto? review)
        {
            if(updateBook == null || bookId != updateBook.Id)
                return BadRequest(ModelState);

            if(!_bookRepository.BookExists(bookId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();

            var bookMap = _mapper.Map<Book>(updateBook);
            var readingTrackingMap = _mapper.Map<ReadingTracking>(readingTracking);
            var reviewMap = _mapper.Map<Review>(review);
            if(!_bookRepository.UpdateBook(catId, statusId, bookMap, readingTrackingMap, reviewMap))
            {
                ModelState.AddModelError("", "bir şeyler ters gitti");
                return StatusCode(500, ModelState);
            }
              
            return Ok("başarıyla güncellendi");
        }

        [HttpDelete("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteBook(int bookId)
        {
            //kitap olup olmasığını kontrol etmek
            if(!_bookRepository.BookExists(bookId))
               return NotFound();

            var bookToDelete = _bookRepository.GetBook(bookId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //nesnenin tamamını silmek
            if(!_bookRepository.DeleteBook(bookToDelete))
                ModelState.AddModelError("", "bir şeyler ters gitti");

            return Ok("başarıyla silindi");
        }
    }
}