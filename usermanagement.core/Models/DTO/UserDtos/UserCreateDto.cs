﻿using System.ComponentModel.DataAnnotations;

namespace usermanagement.core.Models.DTO
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(150)]
        public string? Username { get; set; }

        public string? AzureAdUserId { get; set; }

        [Required]
        [StringLength(150)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(150)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(150)]
        public string? UserRoleId { get; set; }
    }
}
