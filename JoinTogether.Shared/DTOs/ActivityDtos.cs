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

// US-15: full detail view of a single activity, returned by GET api/Activity/{id}
public class ActivityDetailDto
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
    public string CreatedByName { get; set; } = string.Empty;

    // Display names of everyone who has joined (US-15: "Display participants")
    public List<string> Participants { get; set; } = new();
}