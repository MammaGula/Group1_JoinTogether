namespace JoinTogether.DAL.Entities;

public class ActivityParticipant
{
    public int Id { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}