namespace JoinTogether.Shared.DTOs;


// Sent to the client when loading a quiz — no correct-answer info included
public class QuizOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<QuizOptionDto> Options { get; set; } = new();
}

// Sent from the client when submitting answers
public class QuizAnswerDto
{
    public int QuestionId { get; set; }
    public int SelectedOptionId { get; set; }
}


// Sent from the client when submitting a quiz
public class SubmitQuizDto
{
    public int LocationId { get; set; }
    public List<QuizAnswerDto> Answers { get; set; } = new();
}

// Sent back to the client after grading
public class QuizResultDto
{
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double ScorePercentage { get; set; }
    public bool Passed { get; set; }
}