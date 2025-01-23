using System.ComponentModel.DataAnnotations;

namespace MVC_Studio.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }

    }
}
