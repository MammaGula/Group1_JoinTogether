namespace JoinTogether.DAL.Entities;

public class QuizAttempt
{
    public int Id { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public bool Passed { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
}