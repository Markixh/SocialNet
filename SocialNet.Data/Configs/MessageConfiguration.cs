using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNet.Data.Models;

namespace SocialNet.Data.Configs
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        /// <summary>
        /// Конфигурация для таблицы UserMessages
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("UserMessages").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }
}
