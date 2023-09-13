using System.ComponentModel.DataAnnotations;

namespace BazaGljivara.Client.Models.Accounts
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
