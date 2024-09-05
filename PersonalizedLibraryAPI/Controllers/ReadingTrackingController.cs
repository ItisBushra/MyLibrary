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
        private readonly IMapper _mapper;
        public ReadingTrackingController(IReadingTrackingRepository readingTrackingRepository, IMapper mapper)
        {
            _readingTrackingRepository = readingTrackingRepository;
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

            var readingTracking = _mapper.Map<ReadingTrackingDto>(_readingTrackingRepository.GetReadingTracking(readingTrackingId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(readingTracking);
        }

        [HttpGet("book/{readingTrackingId}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        public IActionResult GetBookByReviewId(int readingTrackingId)
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
        public IActionResult GetReviewByBookId(int bookId)
        {
            var readingTracking = _mapper.Map<ReadingTrackingDto>(_readingTrackingRepository.GetReadingTrackingByBook(bookId));

            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(readingTracking);
        }
    }
}