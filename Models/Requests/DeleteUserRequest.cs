using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class DeleteUserRequest
    {
        [Required]
        [MinLength(1)]
        public string requestedBy { get; set; }

        [Required]
        [MinLength(1)]
        public string userLogin { get; set; }

        [Required]
        public bool softDelete { get; set; }
    }
}
