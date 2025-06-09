using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
