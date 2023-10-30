using System.ComponentModel.DataAnnotations;

namespace usermanagement.core.Models.DTO
{
    public class UserUpdateDto
    {
        [Required]
        public string? Id { get; set; }

        [StringLength(150)]
        public string? Username { get; set; }

        [StringLength(150)]
        public string? FirstName { get; set; }

        [StringLength(150)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(300)]
        public string? Email { get; set; }

        [StringLength(150)]
        public string? UserRoleId { get; set; }

        public string? Status { get; set; }
    }
}
