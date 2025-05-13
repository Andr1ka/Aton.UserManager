using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class UpdateUserBirthdayRequest
    {
        [Required]
        public DateTime Birthday { get; set; }
    }
}
