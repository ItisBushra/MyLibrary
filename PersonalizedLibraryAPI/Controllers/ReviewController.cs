using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models;
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Repository.IRepository;
using AutoMapper;

namespace PersonalizedLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if(!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpGet("book/{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(400)]
        public IActionResult GetBookByReviewId(int reviewId)
        {
            var book = _mapper.Map<BookDto>(_reviewRepository.GetBookByReview(reviewId));

            //validation
            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(book);
        }

        [HttpGet("review/{bookId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewByBookId(int bookId)
        {
            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReviewByBook(bookId));

            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(review);
        }
        
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int bookId, [FromBody] ReviewDto reviewCreate)
        {
            //kullanıcı null olarak oluşturulmuşsa
            if(reviewCreate == null || !ModelState.IsValid) 
                return BadRequest(ModelState);

            //review oluştur
            var reviewMap = _mapper.Map<Review>(reviewCreate);
            reviewMap.Book = _bookRepository.GetBook(bookId);

            //kitabın var olup olmadığını kontrol etmek
            if(!_bookRepository.BookExists(bookId))
            {
                ModelState.AddModelError("", "Kitap mevcut değil");
                return StatusCode(422, ModelState);
            }
            //kitabın bir incelemesi olup olmadığını kontrol etmek
            if(_reviewRepository.GetReviewByBook(bookId) != null)
            {
                ModelState.AddModelError("", "mevcut bir inceleme var");
                return StatusCode(422, ModelState);
            }
            //inceleme işlem başarısızlığı yaratır
            if(!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "bir şeyler ters gitti");
                return StatusCode(500, ModelState);
            }
            return Ok("başarıyla oluşturuldu");
        }

    }
}