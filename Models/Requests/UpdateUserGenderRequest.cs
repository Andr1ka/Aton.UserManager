using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserGenderRequest
    {
        [Required]
        [MinLength(1)]
        public string updatedBy { get; set; }

        [Required]
        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }
    }
}
