using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Responses
{
    public class AuthenticatedUserResponse
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public GenderType Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool IsAdmin { get; set; }
    }
} 