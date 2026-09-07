using JoinTogether.DAL.Entities;
 
namespace JoinTogether.DAL.Repositories;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<Location?> GetWithQuizAsync(int locationId);

    //  browse list, needs QuizQuestions eager-loaded for accurate counts
    Task<List<Location>> GetAllWithQuizAsync();
}
