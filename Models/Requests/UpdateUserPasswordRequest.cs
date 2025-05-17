using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's password
    /// </summary>
    public class UpdateUserPasswordRequest
    {
        /// <summary>
        /// New user password. Must be at least 6 characters long and contain only letters and numbers
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string newPassword { get; set; }
    }
} 