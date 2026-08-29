using JoinTogether.DAL.Data;
using JoinTogether.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace JoinTogether.DAL.Repositories;

public class LocationRepository : GenericRepository<Location>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context) { }

    public async Task<Location?> GetWithQuizAsync(int locationId) =>
        await DbSet
            .Include(l => l.QuizQuestions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(l => l.Id == locationId);
}