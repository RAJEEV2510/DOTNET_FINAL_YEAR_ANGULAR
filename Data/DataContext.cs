using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext:IdentityDbContext<AppUser,AppRole,int,IdentityUserClaim<int>,
        AppUserRole,IdentityUserLogin<int>,IdentityRoleClaim<int> ,IdentityUserToken<int>>
    {
      public  DataContext( DbContextOptions options):base(options){}
   
      public DbSet<UserLike> Likes { get; set; }
      public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<AppUserRole>()
        .HasKey(ur => new {ur.UserId ,ur.RoleId });
            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId);


            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);


            builder.Entity<UserLike>().
                HasKey(k => new { k.SourceUserId, k.LikeUserId });
            
            builder.Entity<UserLike>().HasOne(s => s.SourceUser).
                WithMany(l => l.LikedUsers).
                HasForeignKey(s => s.SourceUserId).
                OnDelete(DeleteBehavior.Cascade);


            builder.Entity<UserLike>().HasOne(s => s.LikeUser).
                WithMany(l => l.LikedByUsers).
                HasForeignKey(s => s.LikeUserId).
                OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(u => u.MessagesRecived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(u => u.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);


        }
    
      

        
    }
}