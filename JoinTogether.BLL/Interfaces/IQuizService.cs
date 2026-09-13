using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface IQuizService
{
    // US-06: get quiz questions for a specific location (no correct answers included)
    Task<LocationQuizDto?> GetQuizByLocationIdAsync(int locationId);
}