namespace RwaMovies.DTOs;

public partial class NotificationRequest
{
    public int? Id { get; set; }

    public string ReceiverEmail { get; set; } = null!;

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;
}
