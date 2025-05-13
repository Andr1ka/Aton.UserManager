using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's password
    /// </summary>
    public class UpdateUserPasswordRequest
    {
        /// <summary>
        /// New password. Must be at least 8 characters long and contain at least one uppercase letter, one number and one special character
        /// </summary>
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one number and one special character")]
        public string NewPassword { get; set; }
    }
} 