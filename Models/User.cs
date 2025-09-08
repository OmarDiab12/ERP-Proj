using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models
{
    public class User : BaseEntity
    {
        [Required, MinLength(10)]
        public string FullName { get; set; }
        [Required, MaxLength(20)]
        public string DisplayName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, PasswordPropertyText]
        public string PasswordHash { get; set; }
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
