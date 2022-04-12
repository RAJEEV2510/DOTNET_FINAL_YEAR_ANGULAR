using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Entities;
using API.Helper;
using API.Extensions;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    public class UsersController : BaseApiController
    {

        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoservice;
        public UsersController(IUserRepository repository, IMapper mapper,IPhotoService photoservice)
        {
            _repository = repository;
            _mapper = mapper;
            _photoservice = photoservice;
        }


        //get all member
        
        [HttpGet(Name = "getUsers")]
        public async Task<ActionResult> getUsers([FromQuery] UserParams userParams) {

           userParams.CurrentUserName =  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _repository.GetUserByUsernameAsync(userParams.CurrentUserName);
          
            if(string.IsNullOrEmpty(userParams.gender))
            {
                userParams.gender = user.Gender == "male" ? "female" : "male";
            }
            var users = await _repository.GetMembersAsync(userParams);
            //add in headers 
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize
                , users.TotalCount, users.TotalPages);
            return Ok(users);
        }


        //get user by name
        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> getUserByName(string username)
        {
            return await _repository.GetMemberAsync(username);
        }

        //update user
        [HttpPut]
        public async Task<ActionResult> updateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user =await _repository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _repository.Update(user);

            if(await _repository.SaveAllAsync())
            {
                return NoContent();
            }

            else
            {
                return BadRequest("failed to update the user");
            }
           
        } 

        //add photo
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _repository.GetUserByUsernameAsync(username);

            //it upload on cloud
            var result = await _photoservice.AddPhotoAsync(file);

            if (result.Error != null) 
                return BadRequest("Photo not uploaded");

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            //if photo count is zero make main photo
            if(user.Photos.Count==0)
            {
                photo.IsMain = true;
            }

           // add photo in user 
            user.Photos.Add(photo);

            if(await _repository.SaveAllAsync())
            {
                return CreatedAtRoute("getUsers", _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding Photo");
        }

        //set main photo
       [HttpPut("set-main-photo/{photoId}")]
       public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _repository.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(x => x.Id==photoId);

            if (photo.IsMain) return BadRequest("Already Main Pic");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _repository.SaveAllAsync())
            return Ok(_mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem found in setting main pic");
                 
        }

        //photo delete
        [HttpDelete("photo-delete/{photoId}")]
        public async Task<ActionResult> deletePhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _repository.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
                return NotFound();
            if (photo.IsMain)
                return BadRequest("It is your profile pic");

            if (photo.PublicId != null)
            {         
                var result=   await _photoservice.DeletionPhotoAsync(photo.PublicId);                
            }
            user.Photos.Remove(photo);
            if(await _repository.SaveAllAsync())
            {
                return NoContent();
            }
            return Ok("Photo not deleted");

          
        }
    }

    

}