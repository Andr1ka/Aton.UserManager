using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's name
    /// </summary>
    public class UpdateUserNameRequest
    {
        /// <summary>
        /// New user name. It can contain only Latin and Russian letters
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[а-яА-Яa-zA-Z]+$")]
        public string newName { get; set; }

        
    }
} 