using JoinTogether.BLL.Interfaces;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Services;

public class QuizService : IQuizService
{
    private readonly ILocationRepository _locationRepository;

    public QuizService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    // --- Get quiz questions for a location ---
    public async Task<LocationQuizDto?> GetQuizByLocationIdAsync(int locationId)
    {
        // GetWithQuizAsync already includes QuizQuestions -> Options
        var location = await _locationRepository.GetWithQuizAsync(locationId);

        // Handle missing/invalid location data
        if (location == null)
            return null;

        return new LocationQuizDto
        {
            LocationId = location.Id,
            LocationName = location.Name,
            Questions = location.QuizQuestions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                Text = q.QuestionText, 
                Options = q.Options.Select(o => new QuizOptionDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            }).ToList()
        };
    }
}