using JoinTogether.DAL.Data;
using JoinTogether.DAL.Entities;
using Microsoft.EntityFrameworkCore;
 
namespace JoinTogether.DAL.Repositories;

public class LocationRepository : GenericRepository<Location>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context) { }

    // GET /api/locations/{id}: include the quiz questions and their options for the location
    public async Task<Location?> GetWithQuizAsync(int locationId) =>
        await DbSet
            .Include(l => l.QuizQuestions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(l => l.Id == locationId);

    // GET /api/locations: browse list only needs the question count, not options, so skip ThenInclude
    public async Task<List<Location>> GetAllWithQuizAsync() =>
        await DbSet
            .Include(l => l.QuizQuestions)
            .ToListAsync();
}