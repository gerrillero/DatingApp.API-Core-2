using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this._repo.GetUsers();
            
            var usersToReturn = this._mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this._repo.GetUser(id);
            
            var userToReturn = this._mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto )
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
                
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var userFromRepo = await this._repo.GetUser(id);
            
            if(userFromRepo == null)
                return NotFound($"Cannot find user with ID of {id}");
                
            if (currentUserId != userFromRepo.Id)
                return Unauthorized();
            
            this._mapper.Map(userForUpdateDto, userFromRepo);
            
            if(await this._repo.SaveAll())
                return NoContent();
            
            throw new Exception($"Updating user {id} failed on save ");
            
        }
    }
}