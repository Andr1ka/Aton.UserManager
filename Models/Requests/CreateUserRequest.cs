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
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }

        /// <summary>
        /// User's password. Must be at least 6 characters long and contain only letters and numbers
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }

        /// <summary>
        /// User's name. It can contain only Latin and Russian letters.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[а-яА-Яa-zA-Z]+$")]
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