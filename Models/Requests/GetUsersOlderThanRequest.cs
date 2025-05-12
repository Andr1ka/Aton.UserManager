using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class GetUsersOlderThanRequest
    {

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string requestedBy { get; set; }

        [Required]
        [Range(1, 100)]
        public int age { get; set; }
    }
}
