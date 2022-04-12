
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private DataContext _db;
        public LikesRepository(DataContext db)
        {
            _db = db;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _db.Likes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _db.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(likes => likes.SourceUserId == userId);
                users = likes.Select(like => like.LikeUser);
            }
            else
            {
                likes = likes.Where(likes => likes.LikeUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _db.Users.Include(x => x.LikedByUsers)
                .FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}
