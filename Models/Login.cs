using System.ComponentModel.DataAnnotations;

namespace MVC_Studio.Models
{
    public class Login
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
     
    }
}
