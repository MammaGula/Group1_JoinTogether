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