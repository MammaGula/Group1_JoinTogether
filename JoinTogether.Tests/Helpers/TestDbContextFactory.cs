using JoinTogether.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace JoinTogether.Tests.Helpers;

/// <summary>
/// - Helper file: for creating a fresh in-memory AppDbContext for repository tests.
/// - Each call uses a new database name (Guid) so tests never share state,
/// - even when they run in parallel.
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