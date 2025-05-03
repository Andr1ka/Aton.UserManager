using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class CreateUserRequest
    {
        public string? createdBy { get; set; }

        [Required]
        [MinLength(1)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }

        [Required]
        [MinLength(1)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [RegularExpression(@"^[a-zA-Zà-ÿÀ-ß]+$")]
        public string Name { get; set; }

        [Required]
        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        public bool? IsAdmin { get; set; } = false;
    }
} 