using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalizedLibraryAPI.Data;
using PersonalizedLibraryAPI.Models; 
using PersonalizedLibraryAPI.DTOs;
using PersonalizedLibraryAPI.Repository.IRepository;

namespace PersonalizedLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>
            (_categoryRepository.GetCategories());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if(!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _mapper.Map<CategoryDto>
            (_categoryRepository.GetCategory(categoryId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpGet("book/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [ProducesResponseType(400)]
        public IActionResult GetBookByCategoryId(int categoryId)
        {
            var books = _mapper.Map<List<BookDto>>
            (_categoryRepository.GetBooksByCategory(categoryId));

            //doğrulama
            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(books);
        }

        [HttpGet("category/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategoryByBookId(int bookId)
        {
            var categories = _mapper.Map<List<CategoryDto>>
            (_categoryRepository.GetCategoriesByBook(bookId));

            //validation
            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(categories);
        }
    }
}