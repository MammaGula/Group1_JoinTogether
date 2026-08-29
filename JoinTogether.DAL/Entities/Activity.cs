using System;

namespace JoinTogether.DAL.Entities;

public class Activity
{

	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public DateTime ScheduledAt { get; set; }
	public int MaxParticipants { get; set; }

	public int LocationId { get; set; }
	public Location Location { get; set; } = null!;

	public string CreatedByUserId { get; set; } = string.Empty;
	public ApplicationUser CreatedBy { get; set; } = null!;

	public ICollection<ActivityParticipant> Participants { get; set; } = new List<ActivityParticipant>();

}
