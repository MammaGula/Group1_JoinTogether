using JoinTogether.DAL.Entities;

namespace JoinTogether.Tests.Helpers;

public static class LocationTestData
{
    private static int _questionIdCounter = 1;
    private static int _optionIdCounter = 1;

    public static void ResetCounters()
    {
        _questionIdCounter = 1;
        _optionIdCounter = 1;
    }

    public static Location CreateLocation(
        string name = "Turning Torso",
        int questionCount = 0)
    {
        var location = new Location
        {
            Name = name,
            Description = "A test location in Malmö.",
            Latitude = 55.6136,
            Longitude = 12.9762,
            Category = "Landmark"
        };

        for (var i = 0; i < questionCount; i++)
        {
            location.QuizQuestions.Add(CreateQuestion($"Question {i + 1}"));
        }

        return location;
    }

    public static QuizQuestion CreateQuestion(
        string text = "What year was this built?",
        int optionCount = 2)
    {
        var question = new QuizQuestion
        {
            Id = _questionIdCounter++,
            QuestionText = text
        };

        for (var i = 0; i < optionCount; i++)
        {
            question.Options.Add(new QuizOption
            {
                Id = _optionIdCounter++,
                Text = $"Option {i + 1}",
                IsCorrect = i == 0
            });
        }

        return question;
    }
}