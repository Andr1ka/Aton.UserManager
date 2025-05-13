using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserGenderRequest
    {

        [Required]
        [EnumDataType(typeof(GenderType))]
        public GenderType Gender { get; set; }
    }
}
