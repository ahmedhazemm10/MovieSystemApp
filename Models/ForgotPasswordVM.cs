using System.ComponentModel.DataAnnotations;

namespace MovieSystem.Models
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
