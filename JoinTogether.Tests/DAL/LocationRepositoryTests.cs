using JoinTogether.DAL.Repositories;
using JoinTogether.Tests.Helpers;
using Xunit;

namespace JoinTogether.Tests.DAL;

/// <summary>
/// >> Regression Test 
/// These are integration-style tests against a real EF Core in-memory database
/// (not a mocked repository) because the whole point is to verify that the
/// actual LINQ/.Include() queries behave correctly. Mocking ILocationRepository
/// here would hide exactly the kind of bug we're testing for.
/// </summary>
public class LocationRepositoryTests
{
    // Test 1. Test that GetAllWithQuizAsync eagerly loads the quiz questions for each location
    [Fact]
    public async Task GetAllWithQuizAsync_LoadsQuizQuestionsEagerly()
    {
        // Arrange: create a location with 3 quiz questions in the in-memory database
        await using var context = TestDbContextFactory.Create();
        var location = LocationTestData.CreateLocation(questionCount: 3);
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var repo = new LocationRepository(context);

        // Act: retrieve all locations with their quiz questions
        var results = await repo.GetAllWithQuizAsync();

        // Assert
        // Regression test: before the fix, GetAllLocationsAsync used the generic
        // GetAllAsync() with no .Include(), so QuizQuestions was always empty here.
        Assert.Single(results);
        Assert.Equal(3, results[0].QuizQuestions.Count);
    }


    // Test 2. Test that GetAllWithQuizAsync returns an empty list when there are no locations
    [Fact]
    public async Task GetAllWithQuizAsync_ReturnsEmptyList_WhenNoLocationsExist()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var repo = new LocationRepository(context); 

        // Act
        var results = await repo.GetAllWithQuizAsync();

        // Assert
        Assert.Empty(results);
    }

    // Test 3. Test that GetAllWithQuizAsync: handles locations with no quiz questions
    [Fact]
    public async Task GetAllWithQuizAsync_HandlesLocationsWithNoQuestions()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        context.Locations.Add(LocationTestData.CreateLocation(name: "Empty Location", questionCount: 0));
        await context.SaveChangesAsync();

        var repo = new LocationRepository(context);

        // Act
        var results = await repo.GetAllWithQuizAsync();

        // Assert
        Assert.Single(results);
        Assert.Empty(results[0].QuizQuestions);
    }


    // Test 4. Test that GetWithQuizAsync: eagerly loads the quiz questions + options for a specific location
    [Fact]
    public async Task GetWithQuizAsync_LoadsQuestionsAndOptionsTogether()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var location = LocationTestData.CreateLocation(questionCount: 1);
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var repo = new LocationRepository(context);

        // Act
        var result = await repo.GetWithQuizAsync(location.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result!.QuizQuestions);
        Assert.Equal(2, result.QuizQuestions.First().Options.Count); // default optionCount = 2(LocationTestData.cs)
    }

    // Test 5. Test that GetWithQuizAsync returns null when the location ID does not exist
    [Fact]
    public async Task GetWithQuizAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();
        var repo = new LocationRepository(context);

        // Act
        var result = await repo.GetWithQuizAsync(999);

        // Assert
        Assert.Null(result);
    }
}