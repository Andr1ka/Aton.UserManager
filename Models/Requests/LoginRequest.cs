using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for user authentication
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's login
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }
    }
} 