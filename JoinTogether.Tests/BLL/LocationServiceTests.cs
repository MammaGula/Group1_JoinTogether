using JoinTogether.BLL.Services;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Tests.Helpers;
using Moq;
using Xunit;

namespace JoinTogether.Tests.BLL;

/// <summary>
/// xUnit + Moq unit tests for LocationService.
/// LocationService is tested with a mocked ILocationRepository, since we only
/// care about the entity-to-DTO mapping logic here — the actual EF Core query
/// behavior is covered separately in DAL/LocationRepositoryTests.cs.
/// </summary>
public class LocationServiceTests
{
    private readonly Mock<ILocationRepository> _repoMock = new();
    private readonly LocationService _sut;

    // Constructor: Initialize the SUT (System Under Test) with the mocked repository.
    public LocationServiceTests()
    {
        _sut = new LocationService(_repoMock.Object);
    }

    // Test 1: Verify that GetAllLocationsAsync correctly maps the quiz question count from the entity to the DTO.
    [Fact]
    public async Task GetAllLocationsAsync_MapsQuizQuestionCountCorrectly()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmö C", questionCount: 3);
        location.Id = 1;
        _repoMock.Setup(r => r.GetAllWithQuizAsync())
            .ReturnsAsync(new List<Location> { location });

        // Act
        var result = await _sut.GetAllLocationsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(3, result[0].QuizQuestionCount);
    }


    // Test 2: Verify that GetAllLocationsAsync correctly maps all fields from the entity to the DTO.
    [Fact]
    public async Task GetAllLocationsAsync_MapsAllFieldsCorrectly()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Turning Torso", questionCount: 1);
        location.Id = 5;
        _repoMock.Setup(r => r.GetAllWithQuizAsync())
            .ReturnsAsync(new List<Location> { location });

        // Act
        var result = await _sut.GetAllLocationsAsync();

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(location.Id, dto.Id);
        Assert.Equal(location.Name, dto.Name);
        Assert.Equal(location.Description, dto.Description);
        Assert.Equal(location.Latitude, dto.Latitude);
        Assert.Equal(location.Longitude, dto.Longitude);
        Assert.Equal(location.Category, dto.Category);
    }


    // Test 3: Verify that GetAllLocationsAsync returns an empty list when the repository returns no locations.
    [Fact]
    public async Task GetAllLocationsAsync_ReturnsEmptyList_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllWithQuizAsync())
            .ReturnsAsync(new List<Location>());

        // Act
        var result = await _sut.GetAllLocationsAsync();

        // Assert
        Assert.Empty(result);
    }


    // Test 4: Verify that GetLocationByIdAsync returns null when the location is not found.
    [Fact]
    public async Task GetLocationByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetWithQuizAsync(99))
            .ReturnsAsync((Location?)null);

        // Act
        var result = await _sut.GetLocationByIdAsync(99);

        // Assert
        Assert.Null(result);
    }


    // Test 5: Verify that GetLocationByIdAsync returns the correct DTO when the location is found.
    [Fact]
    public async Task GetLocationByIdAsync_ReturnsCorrectDto_WhenFound()
    {
        // Arrange
        var location = LocationTestData.CreateLocation(name: "Malmöhus Slott", questionCount: 4);
        location.Id = 7;
        _repoMock.Setup(r => r.GetWithQuizAsync(7))
            .ReturnsAsync(location);

        // Act
        var result = await _sut.GetLocationByIdAsync(7);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result!.Id);
        Assert.Equal("Malmöhus Slott", result.Name);
        Assert.Equal(4, result.QuizQuestionCount);
    }


    // Test 6: Verify that GetLocationByIdAsync calls the repository with the correct ID and only once.
    [Fact]
    public async Task GetLocationByIdAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        _repoMock.Setup(r => r.GetWithQuizAsync(It.IsAny<int>()))
            .ReturnsAsync((Location?)null);

        // Act
        await _sut.GetLocationByIdAsync(42);

        // Assert
        _repoMock.Verify(r => r.GetWithQuizAsync(42), Times.Once);
    }
}