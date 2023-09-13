using System.ComponentModel.DataAnnotations;

namespace BazaGljivara.Client.Models.Accounts
{
    public class EditUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [MinLength(6, ErrorMessage = "Polje za lozinku mora biti minimalno od 6 karaktera")]
        public string Password { get; set; }

        public EditUser() { }

        public EditUser(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
        }
    }
}
