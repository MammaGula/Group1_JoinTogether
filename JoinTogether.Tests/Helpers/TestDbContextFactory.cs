using JoinTogether.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace JoinTogether.Tests.Helpers;

/// <summary>
/// - Helper file: for creating a fresh in-memory AppDbContext for repository tests.
/// - Each call uses a new database name (Guid) so tests never share state >> each test gets a fresh database separate.
/// - even when they run in parallel.
/// To see behavior of EF Core in-memory database, like .include(), LINQ queries, etc.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}