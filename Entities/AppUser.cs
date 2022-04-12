using API.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace API.Entities
{
    public class AppUser:IdentityUser<int>
    {
       
        public string knownAs { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime LastActive { get; set; } = DateTime.Now.ToUniversalTime();
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Country { get; set; }
        public string Interest { get; set; }
        public string City { get; set; }
        public ICollection<Photo> Photos { get; set; }

        public ICollection<UserLike> LikedByUsers { get; set; } = new Collection<UserLike>();
        public ICollection<UserLike> LikedUsers { get;set; } = new Collection<UserLike>();

        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessagesRecived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
