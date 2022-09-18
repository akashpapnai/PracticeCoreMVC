using System.ComponentModel.DataAnnotations;

namespace PracticeCoreMVC.Models
{
    public class RoleModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Role { get; set; }
    }
}
