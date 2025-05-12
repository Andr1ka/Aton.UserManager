using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserNameRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string updatedBy { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z�-��-�]+$")]
        public string Name { get; set; }

        
    }
} 