using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class MessageController:BaseApiController
    {
        private IMessageRepository _messageRepository;
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessageController(IUserRepository userRepository,IMessageRepository messageRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createmessagedto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sender = await _userRepository
                .GetUserByUsernameAsync(username);

            if(username==createmessagedto.RecipientUsername.ToLower())
                return BadRequest("You cannot send message to Yourself");

            var recipeint = await _userRepository.
                GetUserByUsernameAsync(createmessagedto.RecipientUsername);

            if (recipeint == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipeint,
                SenderUserName=username,
                ReciverUserName=createmessagedto.RecipientUsername,
                content=createmessagedto.Content,
            };

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllSync())
                return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Message Not Added");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> 
            GetMesagesForUser([FromQuery] MessageParams messageParams )
        {
            messageParams.Username= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize,
                messages.TotalCount,messages.TotalPages);
            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> 
            GetMessageThread(string Username)
        {
            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(await _messageRepository.GetMessageThread(currentUserName, Username));
        }
    }
}
