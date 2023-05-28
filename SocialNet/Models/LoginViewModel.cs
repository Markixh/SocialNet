using SocialNet.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialNet.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
