﻿using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public GenderType Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string? RevokedBy { get; set; }

    }
}
