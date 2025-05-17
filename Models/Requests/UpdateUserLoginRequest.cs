using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's login
    /// </summary>
    public class UpdateUserLoginRequest
    {
        /// <summary>
        /// New user login. Must contain only letters and numbers
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string newLogin { get; set; }
    }
} 