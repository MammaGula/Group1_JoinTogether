namespace JoinTogether.DAL.Entities;

public class QuizQuestion
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}