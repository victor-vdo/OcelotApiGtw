using System.ComponentModel.DataAnnotations;

namespace OcelotApiGtw.Domain.Models
{
    public class AuthUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
