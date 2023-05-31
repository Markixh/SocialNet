using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialNet.Data.Models;
using SocialNet.Data.Repositories;
using SocialNet.Data.UoW;
using SocialNet.Extensions;
using SocialNet.ViewModels.Account;

namespace SocialNet.Controllers
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private IUnitOfWork _unitOfWork;

        public AccountManagerController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Страница для Аутентификаации пользователя
        /// </summary>
        /// <returns></returns>
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Обработка данный для аутентификации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = _mapper.Map<User>(model);

                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View("Views/Home/Index.cshtml");
        }

        /// <summary>
        /// Выход с сайта
        /// </summary>
        /// <returns></returns>
        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Профиль пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var model = new UserViewModel(result);

            model.Friends = await GetAllFriend(model.User);

            return View("User", model);
        }

        /// <summary>
        /// Получить информацию о друзьях в БД
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<User>> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }

        /// <summary>
        /// Страница по редактированию информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var editmodel = _mapper.Map<UserEditViewModel>(result);

            return View("Edit", editmodel);
        }

        /// <summary>
        /// Сохранение ихменений по пользователю
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        /// <summary>
        /// Получить список пользователей по тексту
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(string search)
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var model = await CreateSearch(search);

            if (result == null) return View("UserListNoButton", model);           
            return View("UserList", model);
        }

        /// <summary>
        /// Поиск пользователя
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            if (search.IsNullOrEmpty()) search = "";

            var list = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        /// <summary>
        /// Запрос по добавлению в друзья
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.AddFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        /// <summary>
        /// Запрос по удалению друга
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        /// <summary>
        /// Запрос для отправления сообщения
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var model = await GenerateChat(id);
            return View("Chat", model);
        }

        /// <summary>
        /// Получение сообщений для чата
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);


            var repository = _unitOfWork.GetRepository<Message>() as MessagesRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        /// <summary>
        /// Чат
        /// </summary>
        /// <returns></returns>
        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);
        }
        /// <summary>
        /// Новое сообщение
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chat"></param>
        /// <returns></returns>
        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessagesRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            repository.Create(item);

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var usergen = new GenerateUsers();
            var userlist = usergen.Populate(10);

            foreach (var user in userlist)
            {
                var result = await _userManager.CreateAsync(user, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
