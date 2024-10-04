using System.ComponentModel.DataAnnotations;

namespace todowebapi.Models.DTOs{
    public class UserLoginRequsetDto{
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password {get; set;}
    }
}