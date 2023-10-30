using System.ComponentModel.DataAnnotations;

namespace usermanagement.core.Models.DTO
{
    public class UserRoleUpdateDto
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
