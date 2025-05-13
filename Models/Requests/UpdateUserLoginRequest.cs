using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserLoginRequest
    {

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string NewLogin { get; set; }
    }
} 