using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    internal class UpdateUserBirthdayRequest
    {
        [Required]
        [MinLength(1)]
        public string updatedBy { get; set; }

        [Required]
        public DateTime Birthday { get; set; }
    }
}
