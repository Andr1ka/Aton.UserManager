using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class LoginRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }
    }
} 