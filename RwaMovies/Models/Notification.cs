using System;
using System.Collections.Generic;

namespace RwaMovies.Models;

public partial class Notification
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string ReceiverEmail { get; set; } = null!;

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;

    public DateTime? SentAt { get; set; }
}
