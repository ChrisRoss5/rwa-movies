using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared
{
    public class NotificationRequest
    {
        public int? Id { get; set; }
        [StringLength(256)]
        public string ReceiverEmail { get; set; } = null!;
        [StringLength(256)]
        public string? Subject { get; set; }
        [StringLength(1024)]
        public string Body { get; set; } = null!;
    }
}