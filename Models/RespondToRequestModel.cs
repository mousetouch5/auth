using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auth.Models
{
    public class RespondToRequestModel
    {
        public int FriendshipId { get; set; }
        public string Action { get; set; } // "Accept" or "Decline"
    }
}