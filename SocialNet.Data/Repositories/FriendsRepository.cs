using Microsoft.EntityFrameworkCore;
using SocialNet.Data.Models;

namespace SocialNet.Data.Repositories
{
    /// <summary>
    /// Репозиторий для работы с друзьями
    /// </summary>
    public class FriendsRepository : Repository<Friend>
    {
        public FriendsRepository(ApplicationDbContext db) : base(db)
        {

        }

        /// <summary>
        /// Добавление друга
        /// </summary>
        /// <param name="target">Текущий пользователь</param>
        /// <param name="Friend">Пользователь, для добавления в друзья</param>
        public void AddFriend(User target, User Friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends == null)
            {
                var item = new Friend()
                {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = Friend,
                    CurrentFriendId = Friend.Id,
                };

                Create(item);
            }
        }

        /// <summary>
        /// Получение списка друзей по пользователю
        /// </summary>
        /// <param name="target">Пользователь</param>
        /// <returns></returns>
        public List<User> GetFriendsByUser(User target)
        {
            var friends = Set.Include(x => x.CurrentFriend).AsEnumerable().Where(x => x.User.Id == target.Id).Select(x => x.CurrentFriend);

            return friends.ToList();
        }

        /// <summary>
        /// Удаление друга
        /// </summary>
        /// <param name="target">Текущий пользователь</param>
        /// <param name="Friend">Друг, для удаления</param>
        public void DeleteFriend(User target, User Friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends != null)
            {
                Delete(friends);
            }
        }

    }
}
