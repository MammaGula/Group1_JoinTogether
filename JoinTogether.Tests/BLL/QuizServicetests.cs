using JoinTogether.BLL.Services;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
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
        _sut = new QuizService(_repoMock.Object, _attemptMock.Object);
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
}