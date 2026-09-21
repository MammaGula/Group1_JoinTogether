using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface IQuizService
{

    Task<LocationQuizDto?> GetQuizByLocationIdAsync(int locationId);
    Task<QuizResultDto> SubmitQuizAsync(string userId, SubmitQuizRequest request);
    Task<bool> HasPassedQuizAsync(string userId, int locationId);
}