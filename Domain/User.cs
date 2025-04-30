using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры")]
        public string Login { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя может содержать только русские и латинские буквы")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 2, ErrorMessage = "Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно)")]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; } 

        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; } = string.Empty;

        public DateTime? RevokedOn { get; set; }

        public string RevokedBy { get; set; } = string.Empty;

        public int GetAge()
        {
            if (!Birthday.HasValue) return 0;

            var today = DateTime.Today;
            var age = today.Year - Birthday.Value.Year;
            if (Birthday.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsActive => !RevokedOn.HasValue;
    }
}
