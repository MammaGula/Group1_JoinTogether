using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface IActivityService
{
    // Create an activity at a location (requires having passed that location's quiz)
    Task<ActivityDto> CreateActivityAsync(string userId, CreateActivityDto request);

    // View all activities tied to a location
    Task<List<ActivityDto>> GetActivitiesByLocationIdAsync(int locationId);

    // Join an existing activity
    Task JoinActivityAsync(string userId, int activityId);

    // Full detail view of a single activity (creator name + participant names)
    Task<ActivityDetailDto?> GetActivityByIdAsync(int activityId);
}