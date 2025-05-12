using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class DeleteUserRequest
    {
        [MinLength(1)]
        [MaxLength(50)]
        [Required]
        public string requestedBy { get; set; }

        [Required]
        public bool softDelete { get; set; }
    }
}
