using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hello_Docker_Web.Models
{
    public class UsersModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }


        [Required]
        public int? Age { get; set; }

        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Required]
        [PasswordPropertyText]
        public string? Password { get; set; }

    }
}
