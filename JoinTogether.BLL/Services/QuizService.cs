using JoinTogether.BLL.Interfaces;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Services;

public class QuizService : IQuizService
{
    // Pass threshold constant (75% pass rule from the product spec, 3/4)
    private const double PassThreshold = 0.75;


    
    private readonly ILocationRepository _locationRepository;
    private readonly IGenericRepository<QuizAttempt> _attemptRepository;


   
    public QuizService(
        ILocationRepository locationRepository,
        IGenericRepository<QuizAttempt> attemptRepository)
    {
        _locationRepository = locationRepository;
        _attemptRepository = attemptRepository; // for saving quiz attempts
    }

    // --- 1. Fetch questions based on the selected location ---
    public async Task<List<QuizQuestionDto>?> GetQuestionsByLocationAsync(int locationId)
    {
        var location = await _locationRepository.GetWithQuizAsync(locationId);
        if (location == null)
            return null; // let the controller turn this into a 404


        // 1.1 If the location exists, map the questions and options to DTOs, excluding the IsCorrect property
        return location.QuizQuestions.Select(q => new QuizQuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            Options = q.Options.Select(o => new QuizOptionDto
            {
                Id = o.Id,
                Text = o.Text
                // IsCorrect intentionally omitted(will not be sent to the client)
            }).ToList()
        }).ToList();
    }

    // --- 2 Validate answers and calculate the score ---
    public async Task<QuizResultDto> SubmitQuizAsync(string userId, SubmitQuizDto submission)
    {
        // 2.1 Find the location and its quiz questions
        var location = await _locationRepository.GetWithQuizAsync(submission.LocationId);
        // - If the location doesn't exist, throw an exception (or handle it as you see fit)
        if (location == null)
            throw new KeyNotFoundException("Location not found.");

        // - If the location exists, validate the answers and calculate the score
        var questions = location.QuizQuestions.ToList();
        int correctCount = 0;

        // 2.2 Loop through each question and check the user's answer
        foreach (var question in questions)
        {
            // - Find what the user answered for this question
            var answer = submission.Answers
                .FirstOrDefault(a => a.QuestionId == question.Id);

            if (answer == null)
                continue; // unanswered question = counted as wrong

            // - Validate: does the selected option actually belong to this question, and is it correct?
            var selectedOption = question.Options
                .FirstOrDefault(o => o.Id == answer.SelectedOptionId);

            //- If the selected option is correct, increment the correct count
            if (selectedOption != null && selectedOption.IsCorrect)
                correctCount++;
        }


        // 3. Calculate the total number of questions and the score percentage
        int totalQuestions = questions.Count;
        double scorePercentage = totalQuestions == 0
            ? 0
            : (double)correctCount / totalQuestions * 100;


        // 4. Check if the user passed based on the pass threshold
        bool passed = totalQuestions > 0
            && (double)correctCount / totalQuestions >= PassThreshold;

        // 5. Save the quiz attempt to the database
        var attempt = new QuizAttempt
        {
            UserId = userId,
            LocationId = submission.LocationId,
            CorrectAnswers = correctCount,
            TotalQuestions = totalQuestions,
            Passed = passed
        };

        await _attemptRepository.AddAsync(attempt);
        await _attemptRepository.SaveChangesAsync();


        // 6. Return the result to the client
        return new QuizResultDto
        {
            CorrectAnswers = correctCount,
            TotalQuestions = totalQuestions,
            ScorePercentage = Math.Round(scorePercentage, 2),
            Passed = passed
        };
    }
}
