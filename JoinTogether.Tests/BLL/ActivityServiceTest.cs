using JoinTogether.BLL.Services;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;
using JoinTogether.Tests.Helpers;
using Moq;
using Xunit;
using JoinTogether.BLL.Interfaces;

namespace JoinTogether.Tests.BLL;

/// <summary>
/// xUnit + Moq unit tests for ActivityService.
/// Tests for CreateActivityAsync, GetActivitiesByLocationIdAsync, JoinActivityAsync, GetActivityByIdAsync
/// Quiz gating: users must have passed the quiz for a location before creating or joining activities there.
/// </summary>
///
public class ActivityServiceTests
{
    // mock repositories/services to simulate data access without hitting a real database.
    private readonly Mock<IGenericRepository<Activity>> _activityMock = new();
    private readonly Mock<IGenericRepository<ActivityParticipant>> _participantMock = new();
    private readonly Mock<IGenericRepository<ApplicationUser>> _userMock = new();
    private readonly Mock<ILocationRepository> _locationMock = new();
    private readonly Mock<IQuizService> _quizServiceMock = new();
    private readonly ActivityService _sut;

    public ActivityServiceTests()
    {
        LocationTestData.ResetCounters();
        _sut = new ActivityService(
            _activityMock.Object,
            _participantMock.Object,
            _userMock.Object,
            _locationMock.Object,
            _quizServiceMock.Object); // SUT = System Under Test (ActivityService)
    }

    // --- CreateActivityAsync: quiz gating + missing entities ---

    // Test 1: location does not exist -> throws KeyNotFoundException.
    [Fact]
    public async Task CreateActivityAsync_LocationNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _locationMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Location?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.CreateActivityAsync("user1", new CreateActivityDto { LocationId = 99 }));
    }


    // Test 2: location exists but user has NOT passed its quiz -> throws InvalidOperationException,
    [Fact]
    public async Task CreateActivityAsync_QuizNotPassed_ThrowsInvalidOperationException()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso");
        location.Id = 1;
        _locationMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(location);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user1", 1)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateActivityAsync("user1", new CreateActivityDto { LocationId = 1 }));

        _activityMock.Verify(r => r.AddAsync(It.IsAny<Activity>()), Times.Never);
    }


    // Test 3: location exists and user HAS passed the quiz -> activity is created and saved,
    // and the returned DTO starts at 0 participants.
    [Fact]
    public async Task CreateActivityAsync_QuizPassed_CreatesActivityAndReturnsDto()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso");
        location.Id = 1;
        _locationMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(location);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user1", 1)).ReturnsAsync(true);

        var request = new CreateActivityDto
        {
            Title = "Library tour",
            Description = "Casual meetup",
            ScheduledAt = new DateTime(2026, 10, 1, 18, 0, 0),
            MaxParticipants = 5,
            LocationId = 1
        };

        // Act
        var result = await _sut.CreateActivityAsync("user1", request);

        // Assert
        Assert.Equal("Library tour", result.Title);
        Assert.Equal(1, result.LocationId);
        Assert.Equal("Turning Torso", result.LocationName);
        Assert.Equal("user1", result.CreatedByUserId);
        Assert.Equal(0, result.CurrentParticipants);
        Assert.False(result.IsFull);

        _activityMock.Verify(r => r.AddAsync(It.IsAny<Activity>()), Times.Once);
        _activityMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    // --- GetActivitiesByLocationIdAsync: participant count / full mapping ---

    // Test 4: location does not exist -> returns an empty list (not null, no exception).
    [Fact]
    public async Task GetActivitiesByLocationIdAsync_LocationNotFound_ReturnsEmptyList()
    {
        // Arrange
        _locationMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Location?)null);

        // Act
        var result = await _sut.GetActivitiesByLocationIdAsync(99);

        // Assert
        Assert.Empty(result);
    }


    // Test 5: CurrentParticipants is calculated from actual ActivityParticipant rows,
    // and IsFull becomes true once the count reaches MaxParticipants.
    [Fact]
    public async Task GetActivitiesByLocationIdAsync_CalculatesParticipantCountAndIsFull()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmö C");
        location.Id = 2;
        _locationMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(location);

        var activity = new Activity { Id = 10, Title = "Meetup", LocationId = 2, MaxParticipants = 2, CreatedByUserId = "creator1" };
        _activityMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Activity> { activity });
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>
        {
            new ActivityParticipant { Id = 1, ActivityId = 10, UserId = "userA" },
            new ActivityParticipant { Id = 2, ActivityId = 10, UserId = "userB" }
        });

        // Act
        var result = await _sut.GetActivitiesByLocationIdAsync(2);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(2, dto.CurrentParticipants);
        Assert.True(dto.IsFull); // 2/2 = full
    }


    // Test 6: activities belonging to OTHER locations are filtered out.
    [Fact]
    public async Task GetActivitiesByLocationIdAsync_FiltersByLocationId()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmö C");
        location.Id = 2;
        _locationMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(location);

        _activityMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Activity>
        {
            new Activity { Id = 10, LocationId = 2, MaxParticipants = 5, CreatedByUserId = "u1" },
            new Activity { Id = 11, LocationId = 3, MaxParticipants = 5, CreatedByUserId = "u1" } // different location
        });
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>());

        // Act
        var result = await _sut.GetActivitiesByLocationIdAsync(2);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(10, dto.Id);
    }


    // --- JoinActivityAsync: quiz gating, duplicate joins, full activities ---

    // Test 7: activity does not exist -> throws KeyNotFoundException.
    [Fact]
    public async Task JoinActivityAsync_ActivityNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _activityMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Activity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.JoinActivityAsync("user1", 99));
    }


    // Test 8: activity exists but user has NOT passed the quiz for its location -> throws.
    [Fact]
    public async Task JoinActivityAsync_QuizNotPassed_ThrowsInvalidOperationException()
    {
        // Arrange
        var activity = new Activity { Id = 1, LocationId = 5, MaxParticipants = 10, CreatedByUserId = "creator1" };
        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user1", 5)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.JoinActivityAsync("user1", 1));
    }


    // Test 9: user has already joined this activity -> throws (duplicate join prevention).
    [Fact]
    public async Task JoinActivityAsync_AlreadyJoined_ThrowsInvalidOperationException()
    {
        // Arrange
        var activity = new Activity { Id = 1, LocationId = 5, MaxParticipants = 10, CreatedByUserId = "creator1" };
        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user1", 5)).ReturnsAsync(true);
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>
        {
            new ActivityParticipant { ActivityId = 1, UserId = "user1" } // already joined
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.JoinActivityAsync("user1", 1));

        _participantMock.Verify(r => r.AddAsync(It.IsAny<ActivityParticipant>()), Times.Never);
    }


    // Test 10: activity is already at MaxParticipants -> throws (capacity check).
    [Fact]
    public async Task JoinActivityAsync_ActivityFull_ThrowsInvalidOperationException()
    {
        // Arrange
        var activity = new Activity { Id = 1, LocationId = 5, MaxParticipants = 1, CreatedByUserId = "creator1" };
        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user2", 5)).ReturnsAsync(true);
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>
        {
            new ActivityParticipant { ActivityId = 1, UserId = "someoneElse" } // fills the only spot
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.JoinActivityAsync("user2", 1));
    }


    // Test 11: all checks pass -> a new ActivityParticipant is saved for this user.
    [Fact]
    public async Task JoinActivityAsync_Success_SavesParticipant()
    {
        // Arrange
        var activity = new Activity { Id = 1, LocationId = 5, MaxParticipants = 10, CreatedByUserId = "creator1" };
        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _quizServiceMock.Setup(q => q.HasPassedQuizAsync("user1", 5)).ReturnsAsync(true);
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>());

        ActivityParticipant? captured = null;
        _participantMock
            .Setup(r => r.AddAsync(It.IsAny<ActivityParticipant>()))
            .Callback<ActivityParticipant>(p => captured = p)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.JoinActivityAsync("user1", 1);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal(1, captured!.ActivityId);
        Assert.Equal("user1", captured.UserId);
        _participantMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    // --- GetActivityByIdAsync: creator name + participant name mapping ---

    // Test 12: activity does not exist -> returns null (not an exception).
    [Fact]
    public async Task GetActivityByIdAsync_ActivityNotFound_ReturnsNull()
    {
        // Arrange
        _activityMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Activity?)null);

        // Act
        var result = await _sut.GetActivityByIdAsync(99);

        // Assert
        Assert.Null(result);
    }


    // Test 13: creator's FullName and each participant's FullName are resolved and
    // included in the detail DTO, and CurrentParticipants matches the resolved names.
    [Fact]
    public async Task GetActivityByIdAsync_MapsCreatorNameAndParticipantNames()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso");
        location.Id = 3;
        var activity = new Activity { Id = 1, Title = "Meetup", LocationId = 3, MaxParticipants = 5, CreatedByUserId = "creator1" };

        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _locationMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(location);
        _userMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ApplicationUser>
        {
            new ApplicationUser { Id = "creator1", FullName = "Alice Creator" },
            new ApplicationUser { Id = "participant1", FullName = "Bob Participant" },
            new ApplicationUser { Id = "unrelatedUser", FullName = "Someone Else" }
        });
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>
        {
            new ActivityParticipant { ActivityId = 1, UserId = "participant1" }
        });

        // Act
        var result = await _sut.GetActivityByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice Creator", result!.CreatedByName);
        Assert.Equal("Turning Torso", result.LocationName);
        Assert.Single(result.Participants);
        Assert.Contains("Bob Participant", result.Participants);
        Assert.Equal(1, result.CurrentParticipants);
    }


    // Test 14: when a user has no FullName set, fall back to their Email instead of crashing.
    [Fact]
    public async Task GetActivityByIdAsync_CreatorWithNoFullName_FallsBackToEmail()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso");
        location.Id = 3;
        var activity = new Activity { Id = 1, LocationId = 3, MaxParticipants = 5, CreatedByUserId = "creator1" };

        _activityMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(activity);
        _locationMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(location);
        _userMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ApplicationUser>
        {
            new ApplicationUser { Id = "creator1", FullName = null, Email = "alice@test.com" }
        });
        _participantMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ActivityParticipant>());

        // Act
        var result = await _sut.GetActivityByIdAsync(1);

        // Assert
        Assert.Equal("alice@test.com", result!.CreatedByName);
    }
}