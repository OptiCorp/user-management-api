using System.ComponentModel.DataAnnotations;

namespace usermanagement.core.Models.DTO
{
    public class UserRoleCreateDto
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
