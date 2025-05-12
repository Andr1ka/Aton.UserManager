using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class RestoreUserRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string requestedBy { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string userLogin { get; set; }
    }
}
