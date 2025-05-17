using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for deleting a user
    /// </summary>
    public class DeleteUserRequest
    {
        /// <summary>
        ///type of deletion
        /// </summary>
        [Required]
        public bool softDelete { get; set; }
    }
}
