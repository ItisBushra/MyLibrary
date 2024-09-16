using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Repository.IRepository;
using AutoMapper;

namespace PersonalizedLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingTrackingController : Controller
    {
        private readonly IReadingTrackingRepository _readingTrackingRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public ReadingTrackingController(IReadingTrackingRepository readingTrackingRepository, IMapper mapper, IBookRepository bookRepository)
        {
            _readingTrackingRepository = readingTrackingRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReadingTracking>))]
        public IActionResult ReadingTrackings()
        {
            var readingTrackings = _mapper.Map<List<ReadingTrackingDto>>(_readingTrackingRepository.GetReadingTrackings());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(readingTrackings);
        }

        [HttpGet("{readingTrackingId}")]
        [ProducesResponseType(200, Type = typeof(ReadingTracking))]
        [ProducesResponseType(400)]
        public IActionResult GetReadingTracking(int readingTrackingId)
        {
            if(!_readingTrackingRepository.ReadingTrackingExists(readingTrackingId))
                return NotFound();

            var readingTracking = _mapper.Map<ReadingTrackingDto>
                                    (_readingTrackingRepository.GetReadingTracking(readingTrackingId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(readingTracking);
        }

        [HttpGet("book/{readingTrackingId}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        public IActionResult GetBookByReadingTrackingId(int readingTrackingId)
        { 
            var book = _mapper.Map<BookDto>(_readingTrackingRepository.GetBookByReadingTracking(readingTrackingId));

            //validation
            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(book);
        }

        [HttpGet("readingTracking/{bookId}")]
        [ProducesResponseType(200, Type = typeof(ReadingTracking))]
        [ProducesResponseType(400)]
        public IActionResult GetReadingTrackingByBookId(int bookId)
        {
            var readingTracking = _mapper.Map<ReadingTrackingDto>(_readingTrackingRepository.GetReadingTrackingByBook(bookId));

            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(readingTracking);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReadingTracking([FromQuery] int bookId, [FromBody] ReadingTrackingDto readingTrackingCreate)
        {
            //kullanıcı null olarak oluşturulmuşsa
            if(readingTrackingCreate == null || !ModelState.IsValid) 
                return BadRequest(ModelState);

            //readingTracking oluştur
            var readingTrackingMap = _mapper.Map<ReadingTracking>(readingTrackingCreate);
            readingTrackingMap.Book = _bookRepository.GetBook(bookId);

            //kitabın var olup olmadığını kontrol etmek
            if(!_bookRepository.BookExists(bookId))
            {
                ModelState.AddModelError("", "Kitap mevcut değil");
                return StatusCode(422, ModelState);
            }
            //kitabın bir incelemesi olup olmadığını kontrol etmek
            if(_readingTrackingRepository.GetReadingTrackingByBook(bookId) != null)
            {
                ModelState.AddModelError("", "mevcut bir inceleme var");
                return StatusCode(422, ModelState);
            }
            //inceleme işlem başarısızlığı yaratır
            if(!_readingTrackingRepository.CreateReadingTracking(readingTrackingMap))
            {
                ModelState.AddModelError("", "bir şeyler ters gitti");
                return StatusCode(500, ModelState);
            }
            return Ok("başarıyla oluşturuldu");
        }

        [HttpPut("{readingTrackingId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReadingTracking([FromQuery] int bookId, int readingTrackingId, [FromBody] ReadingTrackingDto updateReadingTracking)
        {
            if(updateReadingTracking == null || readingTrackingId != updateReadingTracking.Id)
                return BadRequest(ModelState);

            if(!_readingTrackingRepository.ReadingTrackingExists(readingTrackingId))
                return NotFound();

            if(!ModelState.IsValid)
                return BadRequest();

            var readingTrackingMap = _mapper.Map<ReadingTracking>(updateReadingTracking);
            readingTrackingMap.Book = _bookRepository.GetBook(bookId);
            //kitabın var olup olmadığını kontrol etmek
            if(!_bookRepository.BookExists(bookId))
            {
                ModelState.AddModelError("", "Kitap mevcut değil");
                return StatusCode(422, ModelState);
            }
            if(!_readingTrackingRepository.UpdateReadingTracking(readingTrackingMap))
            {
                ModelState.AddModelError("", "bir şeyler ters gitti");
                return StatusCode(500, ModelState);
            }
            return Ok("başarıyla güncellendi");
        }

        [HttpDelete("{readingTrackingId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReadingTracking(int readingTrackingId)
        {
            if(!_readingTrackingRepository.ReadingTrackingExists(readingTrackingId))
               return NotFound();

            var readingTrackingToDelete = _readingTrackingRepository.GetReadingTracking(readingTrackingId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_readingTrackingRepository.DeleteReadingTracking(readingTrackingToDelete))
                ModelState.AddModelError("", "bir şeyler ters gitti");

            return Ok("başarıyla silindi");
        }
    }
}