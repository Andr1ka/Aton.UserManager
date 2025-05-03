using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserNameRequest
    {
        [Required]
        [MinLength(1)]
        public string updatedBy { get; set; }

        [Required]
        [MinLength(1)]
        [RegularExpression(@"^[a-zA-Z�-��-�]+$")]
        public string Name { get; set; }

        
    }
} 