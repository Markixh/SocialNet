using System.ComponentModel.DataAnnotations;

namespace SocialNet.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя", Prompt = "Введите Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия", Prompt = "Введите Фамилию")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email", Prompt = "Введите Email")]
        public string EmailReg { get; set; }

        [Required]
        [Display(Name = "Год")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "День")]
        public int Date { get; set; }

        [Required]
        [Display(Name = "Месяц")]
        public int Month { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите Пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
        public string PasswordReg { get; set; }

        [Required]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }            
    }
}
