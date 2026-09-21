using JoinTogether.BLL.Services;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;
using JoinTogether.Tests.Helpers;
using Moq;
using Xunit;

namespace JoinTogether.Tests.BLL;

/// <summary>
/// xUnit + Moq unit tests for QuizService.
/// </summary>
/// 
public class QuizServiceTests
{
    // mock repository to simulate data access without hitting a real database.
    private readonly Mock<ILocationRepository> _repoMock = new();
    private readonly Mock<IGenericRepository<QuizAttempt>> _attemptMock = new();
    private readonly QuizService _sut;

    public QuizServiceTests()
    {
        LocationTestData.ResetCounters();
        _sut = new QuizService(_repoMock.Object, _attemptMock.Object); // SUT = System Under Test(QuizService)
    }

    // Helper: build answers where the first `correctCount` questions are answered
    // correctly and the rest are answered with a wrong option.
    private static List<QuizAnswerDto> BuildAnswers(Location location, int correctCount)
    {
        var answers = new List<QuizAnswerDto>();
        var questions = location.QuizQuestions.ToList();

        for (int i = 0; i < questions.Count; i++)
        {
            var q = questions[i];
            bool answerCorrectly = i < correctCount;

            // - Select the correct option if answerCorrectly is true, otherwise select a wrong option.
            var option = answerCorrectly
                ? q.Options.First(o => o.IsCorrect)
                : q.Options.First(o => !o.IsCorrect);
            // - Create a QuizAnswerDto for this question and selected option.
            answers.Add(new QuizAnswerDto
            {
                QuestionId = q.Id,
                SelectedOptionId = option.Id
            });
        }

        return answers;
    }


    // Test 1: Verify that GetQuizByLocationIdAsync returns null when the location is not found.
    [Fact]
    public async Task GetQuizByLocationIdAsync_ReturnsNull_WhenLocationNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetWithQuizAsync(99))
            .ReturnsAsync((Location?)null);

        // Act
        var result = await _sut.GetQuizByLocationIdAsync(99);

        // Assert
        Assert.Null(result);
    }


    // Test 2: Verify that GetQuizByLocationIdAsync maps the location id and name correctly.
    [Fact]
    public async Task GetQuizByLocationIdAsync_MapsLocationIdAndNameCorrectly()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmöhus Slott", questionCount: 2);
        location.Id = 7;
        _repoMock.Setup(r => r.GetWithQuizAsync(7))
            .ReturnsAsync(location);

        // Act
        var result = await _sut.GetQuizByLocationIdAsync(7);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result!.LocationId);
        Assert.Equal("Malmöhus Slott", result.LocationName);
    }


    // Test 3: Verify that GetQuizByLocationIdAsync maps every question and its options correctly.
    [Fact]
    public async Task GetQuizByLocationIdAsync_MapsQuestionsAndOptionsCorrectly()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso", questionCount: 3);
        location.Id = 1;
        _repoMock.Setup(r => r.GetWithQuizAsync(1))
            .ReturnsAsync(location);

        // Act
        var result = await _sut.GetQuizByLocationIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Questions.Count);

        // - 3.1 : Check if the first question and its options are mapped correctly
        var firstQuestion = result.Questions[0];
        var expectedQuestion = location.QuizQuestions.First();
        Assert.Equal(expectedQuestion.Id, firstQuestion.Id);
        Assert.Equal(expectedQuestion.QuestionText, firstQuestion.Text);
        Assert.Equal(expectedQuestion.Options.Count, firstQuestion.Options.Count);

        // - 3.2 : Check if the first option of the first question is mapped correctly
        var firstOption = firstQuestion.Options[0];
        var expectedOption = expectedQuestion.Options.First();
        Assert.Equal(expectedOption.Id, firstOption.Id);
        Assert.Equal(expectedOption.Text, firstOption.Text);
    }



    // Test 4: Verify that GetQuizByLocationIdAsync never leaks which option is correct —
    // QuizOptionDto must not expose an IsCorrect flag to the client.
    [Fact]
    public async Task GetQuizByLocationIdAsync_DoesNotExposeIsCorrectOnOptions()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmö C", questionCount: 1);
        location.Id = 2;
        _repoMock.Setup(r => r.GetWithQuizAsync(2))
            .ReturnsAsync(location);

        // Act
        var result = await _sut.GetQuizByLocationIdAsync(2);

        // Assert — Check DTO type of the first option of the first question
        // to ensure it does not have an IsCorrect property.
        var optionType = result!.Questions[0].Options[0].GetType();
        Assert.Null(optionType.GetProperty("IsCorrect"));
    }



    // Test 5: Verify that GetQuizByLocationIdAsync returns an empty (not null) Questions
    // list when the location exists but has no quiz questions yet.
    [Fact]
    public async Task GetQuizByLocationIdAsync_ReturnsEmptyQuestionsList_WhenLocationHasNoQuiz()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Nyfiken plats", questionCount: 0);
        location.Id = 3;
        _repoMock.Setup(r => r.GetWithQuizAsync(3))
            .ReturnsAsync(location);

        // Act
        var result = await _sut.GetQuizByLocationIdAsync(3);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result!.Questions);
    }


    // Test 6: Verify that GetQuizByLocationIdAsync calls the repository with the correct id, only once.
    [Fact]
    public async Task GetQuizByLocationIdAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        _repoMock.Setup(r => r.GetWithQuizAsync(It.IsAny<int>()))
            .ReturnsAsync((Location?)null); // we don't care about the return value for this test

        // Act
        await _sut.GetQuizByLocationIdAsync(42);

        // Assert
        _repoMock.Verify(r => r.GetWithQuizAsync(42), Times.Once); // called exactly once with id 42
    }


    // Test 7: 3/4 correct = 75% -> should pass (threshold is 75%).
    [Fact]
    public async Task SubmitQuizAsync_75Percent_ShouldPass()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 4);
        location.Id = 1;
        _repoMock.Setup(r => r.GetWithQuizAsync(1)).ReturnsAsync(location);

        var request = new SubmitQuizRequest
        {
            LocationId = 1,
            Answers = BuildAnswers(location, correctCount: 3)
        };

        // Act
        var result = await _sut.SubmitQuizAsync("user1", request);

        // Assert
        Assert.Equal(4, result.TotalQuestions);
        Assert.Equal(3, result.CorrectAnswers);
        Assert.Equal(75.0, result.ScorePercent);
        Assert.True(result.Passed);
    }


    // Test 8: 4/4 correct = 100% -> should pass.
    [Fact]
    public async Task SubmitQuizAsync_100Percent_ShouldPass()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 4);
        location.Id = 2;
        _repoMock.Setup(r => r.GetWithQuizAsync(2)).ReturnsAsync(location);

        var request = new SubmitQuizRequest
        {
            LocationId = 2,
            Answers = BuildAnswers(location, correctCount: 4)
        };

        // Act
        var result = await _sut.SubmitQuizAsync("user1", request);

        // Assert
        Assert.Equal(100.0, result.ScorePercent);
        Assert.True(result.Passed);
    }


    // Test 9: 2/3=66.7% → Passed: false
    [Fact]
    public async Task SubmitQuizAsync_JustBelow75Percent_ShouldNotPass()
    {
        // Arrange: 2/3 = 66.7% < 75%
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 3);
        location.Id = 3;
        _repoMock.Setup(r => r.GetWithQuizAsync(3)).ReturnsAsync(location);

        var request = new SubmitQuizRequest
        {
            LocationId = 3,
            Answers = BuildAnswers(location, correctCount: 2)
        };

        // Act
        var result = await _sut.SubmitQuizAsync("user1", request);

        // Assert
        Assert.Equal(66.7, result.ScorePercent);
        Assert.False(result.Passed);
    }


    // Test 10: missing answers for some questions -> counted as wrong, no exception.
    [Fact]
    public async Task SubmitQuizAsync_MissingAnswers_CountAsWrong()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 3);
        location.Id = 4;
        _repoMock.Setup(r => r.GetWithQuizAsync(4)).ReturnsAsync(location);

        var request = new SubmitQuizRequest
        {
            LocationId = 4,
            Answers = new() // no answers submitted
        };

        // Act
        var result = await _sut.SubmitQuizAsync("user1", request);

        // Assert
        Assert.Equal(3, result.TotalQuestions);
        Assert.Equal(0, result.CorrectAnswers);
        Assert.Equal(0.0, result.ScorePercent);
        Assert.False(result.Passed);
    }


    // Test 11: location not found -> throws KeyNotFoundException.
    [Fact]
    public async Task SubmitQuizAsync_LocationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetWithQuizAsync(99)).ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.SubmitQuizAsync("user1", new SubmitQuizRequest { LocationId = 99 }));
    }


    // Test 12: a successful submission always saves a QuizAttempt, dont care about pass/not pass
    [Fact]
    public async Task SubmitQuizAsync_SavesQuizAttempt()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 2);
        location.Id = 5;
        _repoMock.Setup(r => r.GetWithQuizAsync(5)).ReturnsAsync(location);

        // Act
        await _sut.SubmitQuizAsync("user1", new SubmitQuizRequest
        {
            LocationId = 5,
            Answers = BuildAnswers(location, correctCount: 1)
        });

        // Assert
        _attemptMock.Verify(r => r.AddAsync(It.IsAny<QuizAttempt>()), Times.Once); // Verify that AddAsync was called once
        _attemptMock.Verify(r => r.SaveChangesAsync(), Times.Once); // Verify that SaveChangesAsync was called once
    }


    // Test 13: the saved QuizAttempt has field values matching the computed result before saving,
    // including UserId, LocationId, CorrectAnswers, TotalQuestions, and Passed.
    [Fact]
    public async Task SubmitQuizAsync_SavedAttempt_HasCorrectFieldValues()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Test", questionCount: 4);
        location.Id = 6;
        _repoMock.Setup(r => r.GetWithQuizAsync(6)).ReturnsAsync(location);

        QuizAttempt? capturedAttempt = null;
        _attemptMock
            .Setup(r => r.AddAsync(It.IsAny<QuizAttempt>()))
            .Callback<QuizAttempt>(a => capturedAttempt = a)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SubmitQuizAsync("user42", new SubmitQuizRequest
        {
            LocationId = 6,
            Answers = BuildAnswers(location, correctCount: 3)
        });

        // Assert
        Assert.NotNull(capturedAttempt);
        Assert.Equal("user42", capturedAttempt!.UserId);
        Assert.Equal(6, capturedAttempt.LocationId);
        Assert.Equal(3, capturedAttempt.CorrectAnswers);
        Assert.Equal(4, capturedAttempt.TotalQuestions);
        Assert.True(capturedAttempt.Passed);
    }


    // --- HasPassedQuizAsync coverage ---

    // Test 14: user has a passed attempt for this exact location -> true.
    [Fact]
    public async Task HasPassedQuizAsync_UserPassedThisLocation_ReturnsTrue()
    {
        // Arrange
        _attemptMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuizAttempt>
        {
            new QuizAttempt { UserId = "user1", LocationId = 1, Passed = true }
        });

        // Act
        var result = await _sut.HasPassedQuizAsync("user1", 1);

        // Assert
        Assert.True(result);
    }


    // Test 15: user has an attempt for this location, but it didn't pass -> false.
    [Fact]
    public async Task HasPassedQuizAsync_UserFailedThisLocation_ReturnsFalse()
    {
        // Arrange
        _attemptMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuizAttempt>
        {
            new QuizAttempt { UserId = "user1", LocationId = 1, Passed = false }
        });

        // Act
        var result = await _sut.HasPassedQuizAsync("user1", 1);

        // Assert
        Assert.False(result);
    }


    // Test 16: a DIFFERENT user passed this location -> should not count for user1.
    [Fact]
    public async Task HasPassedQuizAsync_DifferentUserPassed_ReturnsFalse()
    {
        // Arrange
        _attemptMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuizAttempt>
        {
            new QuizAttempt { UserId = "user2", LocationId = 1, Passed = true }
        });

        // Act
        var result = await _sut.HasPassedQuizAsync("user1", 1);

        // Assert
        Assert.False(result);
    }


    // Test 17: user passed a DIFFERENT location -> should not count for location 1.
    [Fact]
    public async Task HasPassedQuizAsync_PassedDifferentLocation_ReturnsFalse()
    {
        // Arrange
        _attemptMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuizAttempt>
        {
            new QuizAttempt { UserId = "user1", LocationId = 2, Passed = true }
        });

        // Act
        var result = await _sut.HasPassedQuizAsync("user1", 1);

        // Assert
        Assert.False(result);
    }


    // Test 18: no attempts recorded at all -> false, no exception.
    [Fact]
    public async Task HasPassedQuizAsync_NoAttempts_ReturnsFalse()
    {
        // Arrange
        _attemptMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuizAttempt>());

        // Act
        var result = await _sut.HasPassedQuizAsync("user1", 1);

        // Assert
        Assert.False(result);
    }
}