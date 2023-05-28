using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNet.Data.Models;

namespace SocialNet.Data
{    
    public class SocialNetContext : IdentityDbContext<User>
    {
        public SocialNetContext(DbContextOptions<SocialNetContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
