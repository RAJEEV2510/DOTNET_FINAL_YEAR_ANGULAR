
using API.DTOs;
using API.Entities;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public MessageRepository(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                 .OrderByDescending(m => m.MessageSent).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
                "Outbox"=>query.Where(u=>u.Sender.UserName==messageParams.Username),
                _=> query.Where(u =>
                u.Recipient.UserName == messageParams.Username && u.DateRead==null)
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
                .Include(u=>u.Sender).ThenInclude(u=>u.Photos)
                .Include(u=>u.Recipient).ThenInclude(u=>u.Photos)
                .Where(u => u.ReciverUserName == recipientUserName && u.SenderUserName == currentUserName
                || u.ReciverUserName == currentUserName && u.SenderUserName == recipientUserName
                )
                .OrderBy(m=>m.MessageSent).ToListAsync();

            var unreadMessage =messages
                .Where(u => u.DateRead == null && u.Recipient.UserName == currentUserName).ToList();
            if(unreadMessage.Any())
            {
                foreach(var message in unreadMessage)
                {
                    message.DateRead = DateTime.Now.ToUniversalTime();
                }
                await _context.SaveChangesAsync();
            }

            var ListOfMessagesRequiredFormat = new List<MessageDto>();

            foreach(var message in messages)
            {
                ListOfMessagesRequiredFormat.Add(_mapper.Map<MessageDto>(message));
            }

            return ListOfMessagesRequiredFormat;
            
        }

        public async Task<bool> SaveAllSync()
        {
            return await _context.SaveChangesAsync()>0;
        }
    }
}
