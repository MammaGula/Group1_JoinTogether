namespace JoinTogether.Shared.DTOs;

// Sent by the client when creating a new activity at a location.
public class CreateActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int MaxParticipants { get; set; }
    public int LocationId { get; set; }
}

// Returned by GET api/Activity/location/{locationId} and POST api/Activity
public class ActivityDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public bool IsFull { get; set; }

    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
}