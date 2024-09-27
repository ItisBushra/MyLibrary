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
    public class StatusController : Controller
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMapper _mapper;
        public StatusController(IStatusRepository statusRepository, IMapper mapper)
        {
            _mapper = mapper;
            _statusRepository = statusRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Status>))]
        public IActionResult GetStatuses()
        {
            var statuses = _mapper.Map<List<StatusDto>>(_statusRepository.GetStatuses());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(statuses);
        }

        [HttpGet("{statusId}")]
        [ProducesResponseType(200, Type = typeof(Status))]
        [ProducesResponseType(400)]
        public IActionResult GetStatus(int statusId)
        {
            if(!_statusRepository.StatusExists(statusId))
                return NotFound();

            var status = _mapper.Map<StatusDto>(_statusRepository.GetStatus(statusId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(status);
        }

        [HttpGet("/books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(Status))]
        [ProducesResponseType(400)]
        public IActionResult GetStatusOfABook(int bookId)
        {
            var status = _mapper.Map<StatusDto>(_statusRepository.GetStatusByBook(bookId));

            if(!ModelState.IsValid)
               return BadRequest();

            return Ok(status);
        }
    }
}