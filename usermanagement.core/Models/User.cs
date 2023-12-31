﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace usermanagement.core.Models
{
    public enum UserStatus
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Disabled")]
        Disabled,
        [Display(Name = "Deleted")]
        Deleted,
    }
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }


        public string? AzureAdUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string? UserRoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        public UserRole? UserRole { get; }

        [Required]
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
