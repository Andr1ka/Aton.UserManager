using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for creating a new user
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// User's login. Must contain only letters and numbers
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Login must contain only letters and numbers")]
        public string Login { get; set; }

        /// <summary>
        /// User's password. Must be at least 8 characters long and contain at least one uppercase letter, one number and one special character
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one number and one special character")]
        public string Password { get; set; }

        /// <summary>
        /// User's name. Can contain letters, spaces and hyphens
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[а-яА-Яa-zA-Z\s-]+$", ErrorMessage = "Name can contain only letters, spaces and hyphens")]
        public string Name { get; set; }

        /// <summary>
        /// User's gender
        /// </summary>
        [Required]
        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }

        /// <summary>
        /// User's birthday
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Whether the user should have admin privileges
        /// </summary>
        [Required]
        public bool IsAdmin { get; set; } = false;
    }
}