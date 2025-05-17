using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's gender
    /// </summary>
    public class UpdateUserGenderRequest
    {
        /// <summary>
        /// New user gender (0 - female, 1 - male, 2 - unknown)
        /// </summary>
        [Required]
        [EnumDataType(typeof(GenderType))]
        public GenderType newGender { get; set; }
    }
}
