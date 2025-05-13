using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class DeleteUserRequest
    {
        [Required]
        public bool softDelete { get; set; }
    }
}
