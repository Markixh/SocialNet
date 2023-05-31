using Microsoft.EntityFrameworkCore;
using SocialNet.Data.Models;

namespace SocialNet.Data.Repositories
{
    /// <summary>
    /// Репозиторий для обработки сообщений
    /// </summary>
    public class MessagesRepository : Repository<Message>
    {
        public MessagesRepository(ApplicationDbContext db) : base(db)
        {

        }

        /// <summary>
        /// Получение сообщений
        /// </summary>
        /// <param name="sender">Получатель сообщений</param>
        /// <param name="recipient">Отправитель сообщений</param>
        /// <returns></returns>
        public List<Message> GetMessages(User sender, User recipient)
        {
            Set.Include(x => x.Recipient);
            Set.Include(x => x.Sender);

            var from = Set.AsEnumerable().Where(x => x.SenderId == sender.Id && x.RecipientId == recipient.Id).ToList();
            var to = Set.AsEnumerable().Where(x => x.SenderId == recipient.Id && x.RecipientId == sender.Id).ToList();

            var itog = new List<Message>();
            itog.AddRange(from);
            itog.AddRange(to);
            itog.OrderBy(x => x.Id);
            return itog;
        }
    }
}
