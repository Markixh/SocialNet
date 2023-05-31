using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNet.Data.Models;

namespace SocialNet.Data.Configs
{
    public class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
        /// <summary>
        /// Конфигурация для таблицы UserFriends
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable("UserFriends").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }
}
