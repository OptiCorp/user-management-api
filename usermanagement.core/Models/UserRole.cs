using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace usermanagement.core.Models
{
    public class UserRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(50)]
        public string? Id { get; set; }

        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

    }
}
