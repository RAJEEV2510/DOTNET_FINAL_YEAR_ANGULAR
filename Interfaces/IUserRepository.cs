using API.DTOs;
using API.Entities;
using API.Helper;

namespace API.Interfaces
{
    public interface IUserRepository
    {
       public void Update(AppUser user);
       public  Task<bool> SaveAllAsync();
       public  Task<IEnumerable<AppUser>> GetUserAsync();
       public    Task<AppUser> GetUserByIdAsync(int id);
       public   Task<AppUser> GetUserByUsernameAsync(string username);
        //paged List
       public Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        public Task<MemberDto> GetMemberAsync(string username);
    }

}
