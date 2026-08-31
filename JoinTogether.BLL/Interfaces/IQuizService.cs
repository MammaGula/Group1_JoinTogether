using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface IQuizService
{
    // Fetch questions for a given location (options only, no correct-answer flag)
    Task<List<QuizQuestionDto>?> GetQuestionsByLocationAsync(int locationId);

    // Validate submitted answers and calculate + persist the score
    Task<QuizResultDto> SubmitQuizAsync(string userId, SubmitQuizDto submission);
}
