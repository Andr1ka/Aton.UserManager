using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserPasswordRequest
    {
        [Required]
        [MinLength(1)]
        public string updatedBy { get; set; }

        [Required]
        [MinLength(1)]
        public string NewPassword { get; set; }
    }
} 