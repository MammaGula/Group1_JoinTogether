using JoinTogether.BLL.Interfaces;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Services;

public class QuizService : IQuizService
{
    private const double PassThreshold = 75.0; // percent

    private readonly ILocationRepository _locationRepo;
    private readonly IGenericRepository<QuizAttempt> _attemptRepo;

    public QuizService(ILocationRepository locationRepo, IGenericRepository<QuizAttempt> attemptRepo)
    {
        _locationRepo = locationRepo ?? throw new ArgumentNullException(nameof(locationRepo));
        _attemptRepo = attemptRepo ?? throw new ArgumentNullException(nameof(attemptRepo));
    }

    // --- Get quiz questions for a location ---
    public async Task<LocationQuizDto?> GetQuizByLocationIdAsync(int locationId)
    {
        // GetWithQuizAsync already includes QuizQuestions -> Options
        var location = await _locationRepo.GetWithQuizAsync(locationId);

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


    // --- Submit quiz answers and calculate score ---
    public async Task<QuizResultDto> SubmitQuizAsync(string userId, SubmitQuizRequest request)
    {
        // 1. Get the location and its quiz questions
        var location = await _locationRepo.GetWithQuizAsync(request.LocationId);
        if (location == null)
            throw new KeyNotFoundException("Location not found");

        var questions = location.QuizQuestions;
        // 2.Count total questions and correct answers
        int total = questions.Count;
        int correct = 0;

        // 2.1 Per-question feedback (user should see which answers were right/wrong)
        var questionResults = new List<QuestionResultDto>();

        // 3. Loop through each question and check if the submitted answer is correct
        foreach (var q in questions)
        {
            var submitted = request.Answers.FirstOrDefault(a => a.QuestionId == q.Id);

            // 3.1 Find the correct option for the question
            var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);

            bool isCorrect = submitted != null
                && correctOption != null
                && submitted.SelectedOptionId == correctOption.Id;

            // 3.2 Increment correct count if the answer is correct
            if (isCorrect)
                correct++;
            // 3.3 Add the result for this question to the feedback list
            questionResults.Add(new QuestionResultDto
            {
                QuestionId = q.Id,
                IsCorrect = isCorrect,
                SelectedOptionId = submitted?.SelectedOptionId ?? 0, // 0 = unanswered
                CorrectOptionId = correctOption?.Id ?? 0
            });
        }

        // 4. Calculate percentage score and determine pass/fail
        double percent = total == 0 ? 0 : Math.Round((double)correct / total * 100, 1);
        bool passed = percent >= PassThreshold;

        // 5. Record the attempt in the database
        var attempt = new QuizAttempt
        {
            UserId = userId,
            LocationId = request.LocationId,
            CorrectAnswers = correct,
            TotalQuestions = total,
            Passed = passed
        };

        await _attemptRepo.AddAsync(attempt);
        await _attemptRepo.SaveChangesAsync();

        // 6. Return the result DTO
        return new QuizResultDto
        {
            TotalQuestions = total,
            CorrectAnswers = correct,
            ScorePercent = percent,
            Passed = passed,
            Questions = questionResults
        };
    }



    // --- Check whether a user has already passed a location's quiz (used to gate Activity creation/joining) ---
    public async Task<bool> HasPassedQuizAsync(string userId, int locationId)
    {
        var attempts = await _attemptRepo.GetAllAsync();
        return attempts.Any(a => a.UserId == userId && a.LocationId == locationId && a.Passed);
    }
}