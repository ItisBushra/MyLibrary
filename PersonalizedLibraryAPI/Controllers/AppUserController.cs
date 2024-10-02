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
using PersonalizedLibraryAPI.DTOs.Account;

namespace PersonalizedLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAppUserRepository _appUserRepository;
        public AppUserController(IAppUserRepository appUserRepository, IMapper mapper)
        {
            _mapper = mapper;
            _appUserRepository = appUserRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NewUserDto>))]
        public IActionResult GetAppUsers()
        {
            var users =_appUserRepository.GetAppUsers(); 
            var userDto = _mapper.Map<List<NewUserDto>>(users); 

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userDto);
        }
        [HttpGet("{appUserId}")]
        [ProducesResponseType(200, Type = typeof(AppUser))]
        [ProducesResponseType(400)]
        public IActionResult GetAppUser(string appUserId)
        {
            if(!_appUserRepository.AppUserExists(appUserId))
                return NotFound();

            var user = _mapper.Map<LoginDto>
            (_appUserRepository.GetAppUser(appUserId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }
        [HttpGet("user/books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(AppUser))]
        [ProducesResponseType(400)]
        public IActionResult GetUserOfABook(int bookId)
        {
            var user = _mapper.Map<LoginDto>
            (_appUserRepository.GetAppUserByBook(bookId));

            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(user);
        }
    }
}