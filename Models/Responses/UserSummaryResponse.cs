using Domain.Enums;

namespace Models.Responses
{
    public class UserSummaryResponse
    {
        public string Login { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public GenderType Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedOn { get; set; }
    }
} 