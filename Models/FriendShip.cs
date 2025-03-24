using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auth.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Sender

        [Required]
        public string FriendId { get; set; } // Receiver

        [Required]
        [MaxLength(10)]
        public string Status { get; set; } = "Pending"; // "Pending", "Accepted", "Declined"

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("FriendId")]
        public virtual ApplicationUser Friend { get; set; }
    }
}
