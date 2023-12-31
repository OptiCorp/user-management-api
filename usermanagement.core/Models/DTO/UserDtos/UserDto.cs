﻿using System.ComponentModel.DataAnnotations;

namespace usermanagement.core.Models.DTO
{
    public class UserDto
    {
        public string? Id { get; set; }

        public string? AzureAdUserId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Username { get; set; }

        public UserRole? UserRole { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}

