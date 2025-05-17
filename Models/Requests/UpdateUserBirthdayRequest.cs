using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    /// <summary>
    /// Request model for updating user's birthday
    /// </summary>
    public class UpdateUserBirthdayRequest
    {
        /// <summary>
        /// New user birthday date.
        /// </summary>
        public DateTime newBirthday { get; set; }
    }
}
