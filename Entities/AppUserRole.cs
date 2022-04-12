

using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole:IdentityUserRole<int>
    {
       override public int UserId { get; set; }
        public AppUser User { get; set; }
        override public int RoleId { get; set; }
        public AppRole Role { get; set; }
    }
}
