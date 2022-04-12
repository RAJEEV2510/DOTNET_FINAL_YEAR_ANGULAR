
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Entities;
using System.Security.Claims;
using API.DTOs;
using System.Collections.ObjectModel;

namespace API.Controllers
{
    [Authorize]
    public class LikeController:BaseApiController
    {
        private IUserRepository  _userRepository;
        private ILikesRepository _likesRepository;
        public LikeController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(sourceUserName);

            var sourceUserId = user.Id;//source user id

            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();
            if (sourceUser.UserName == username)
                return BadRequest("You Cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null)
                return BadRequest("You already liked this user");

            userLike = new UserLike
            {
               
                SourceUserId = sourceUserId,
                LikeUserId = likedUser.Id,
              
            };

            if (sourceUser.LikedUsers == null)
            {
                sourceUser.LikedUsers = new Collection<UserLike>();
                sourceUser.LikedUsers.Add(userLike);
                await _userRepository.SaveAllAsync();

                return Ok("added like successfully");
            }

            else
            {
                sourceUser.LikedUsers.Add(userLike);
                await _userRepository.SaveAllAsync();
                return Ok("add like succesfully");
            }
         
        }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLike(string predicate)
        {

            var sourceUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(sourceUserName);
            var users= await _likesRepository.GetUserLikes(predicate, user.Id);

            return Ok(users);
        }
    }
}
