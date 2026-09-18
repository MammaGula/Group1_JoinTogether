namespace JoinTogether.Shared.DTOs;

// Sent to the client when a location's quiz is requested.
// Does NOT include which option is correct.
public class QuizOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<QuizOptionDto> Options { get; set; } = new();
}


// Returned by GET api/Quiz/location/{locationId}
public class LocationQuizDto
{
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public List<QuizQuestionDto> Questions { get; set; } = new();
}


// Sent by the client when submitting quiz answers.
public class QuizAnswerDto
{
    public int QuestionId { get; set; }
    public int SelectedOptionId { get; set; }
}


public class SubmitQuizRequest
{
    public int LocationId { get; set; }
    public List<QuizAnswerDto> Answers { get; set; } = new();
}

// Per-question feedback, included in QuizResultDto.
// Only sent AFTER submission, so revealing CorrectOptionId here is safe —
// it does not leak the answer before the user has committed to their choice.
public class QuestionResultDto
{
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public int SelectedOptionId { get; set; }
    public int CorrectOptionId { get; set; }
}

// Returned by POST api/Quiz/submit
public class QuizResultDto
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double ScorePercent { get; set; }
    public bool Passed { get; set; }
    public List<QuestionResultDto> Questions { get; set; } = new();
}