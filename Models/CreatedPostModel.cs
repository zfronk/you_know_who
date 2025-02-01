using System.ComponentModel.DataAnnotations;

namespace Hello_Docker_Web.Models
{
    public class CreatedPostModel
    {
        [Key]
        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string PostTitle { get; set; }

        [Required]
        public string PostBody {  get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }
}

