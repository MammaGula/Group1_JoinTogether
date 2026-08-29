using Microsoft.AspNetCore.Identity;

namespace JoinTogether.DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Activity> CreatedActivities { get; set; } = new List<Activity>();
    public ICollection<ActivityParticipant> ActivityParticipations { get; set; } = new List<ActivityParticipant>();
}