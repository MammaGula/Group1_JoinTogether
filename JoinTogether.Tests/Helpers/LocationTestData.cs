using JoinTogether.DAL.Entities;

namespace JoinTogether.Tests.Helpers;

/// <summary>
/// Static helper: Builders for Location-related test entities, so individual test files
/// Create objects of Location, QuizQuestion, and QuizOption 
/// don't each hand-roll object graphs.
/// </summary>
public static class LocationTestData
{
    // 1. Create a Location with a specified number of quiz questions
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
    

    // 2. Create a QuizQuestion with a specified number of options
    public static QuizQuestion CreateQuestion(
        string text = "What year was this built?",
        int optionCount = 2)
    {
        var question = new QuizQuestion { QuestionText = text };

        for (var i = 0; i < optionCount; i++)
        {
            question.Options.Add(new QuizOption
            {
                Text = $"Option {i + 1}",
                IsCorrect = i == 0 // first option is correct by default
            });
        }

        return question;
    }
}