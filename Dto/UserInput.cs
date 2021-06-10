using System.ComponentModel.DataAnnotations;

namespace Authentication.Dto
{
    public class UserInput
    {
        protected UserInput()
        {
            
        }
        public UserInput(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [EmailAddress]
        [MinLength(1)]
        public string Email { get; set; }

        [MinLength(7)]
        public string Password { get; set; }
    }
}