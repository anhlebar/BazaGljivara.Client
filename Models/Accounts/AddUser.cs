using System.ComponentModel.DataAnnotations;

namespace BazaGljivara.Client.Models.Accounts
{
    public class AddUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Zaporka mora biti minimalno 6 znakova")]
        public string Password { get; set; }
    }
}