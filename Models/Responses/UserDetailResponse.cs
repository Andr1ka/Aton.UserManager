using Domain.Enums;

namespace Models.Responses
{
    public class UserDetailResponse
    {
        public string Name { get; set; }

        public GenderType Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool IsActive { get; set; }
    }
} 