namespace JoinTogether.DAL.Entities;

public class QuizOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public int QuizQuestionId { get; set; }
    public QuizQuestion QuizQuestion { get; set; } = null!;
}