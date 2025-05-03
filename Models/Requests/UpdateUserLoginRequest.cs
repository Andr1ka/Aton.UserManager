using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserLoginRequest
    {
        [Required]
        [MinLength(1)]
        public string updatedBy { get; set; }

        [Required]
        [MinLength(1)]
        public string NewLogin { get; set; }
    }
} 