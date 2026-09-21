using JoinTogether.BLL.Interfaces;
using JoinTogether.DAL.Entities;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Services;

public class ActivityService : IActivityService
{
    private readonly IGenericRepository<Activity> _activityRepo; // Manages Activity entities in the database
    private readonly IGenericRepository<ActivityParticipant> _participantRepo; // Manages ActivityParticipant entities in the database
    private readonly ILocationRepository _locationRepo; // Manages Location entities in the database, including quiz questions and options
    private readonly IQuizService _quizService;  // Provides quiz-related functionality, such as checking if a user has passed a quiz for a specific location
    private readonly IGenericRepository<ApplicationUser> _userRepo;

    // Constructor to initialize the ActivityService with required repositories and services
    public ActivityService(
    IGenericRepository<Activity> activityRepo,
    IGenericRepository<ActivityParticipant> participantRepo,
    IGenericRepository<ApplicationUser> userRepo,        
    ILocationRepository locationRepo,
    IQuizService quizService)
    {
        _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
        _participantRepo = participantRepo ?? throw new ArgumentNullException(nameof(participantRepo));
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));   // 
        _locationRepo = locationRepo ?? throw new ArgumentNullException(nameof(locationRepo));
        _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
    }

    // 1. Create an activity at a location (gated behind the 75% quiz pass, per PBI #5) ---
    public async Task<ActivityDto> CreateActivityAsync(string userId, CreateActivityDto request)
    {
        var location = await _locationRepo.GetByIdAsync(request.LocationId);
        if (location == null)
            throw new KeyNotFoundException("Location not found");

        // - Check if the user has passed the quiz for this location
        var hasPassed = await _quizService.HasPassedQuizAsync(userId, request.LocationId);
        if (!hasPassed)
            throw new InvalidOperationException("You must pass this location's quiz (75%+) before creating an activity here");

        // - Create the activity object and save it to the database
        var activity = new Activity
        {
            Title = request.Title,
            Description = request.Description,
            ScheduledAt = request.ScheduledAt,
            MaxParticipants = request.MaxParticipants,
            LocationId = request.LocationId,
            CreatedByUserId = userId
        };

        await _activityRepo.AddAsync(activity);
        await _activityRepo.SaveChangesAsync();

        // - Return the created activity as a DTO
        return new ActivityDto
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            ScheduledAt = activity.ScheduledAt,
            MaxParticipants = activity.MaxParticipants,
            CurrentParticipants = 0,
            IsFull = activity.MaxParticipants <= 0,
            LocationId = location.Id,
            LocationName = location.Name,
            CreatedByUserId = activity.CreatedByUserId
        };
    }

    // 2. View all activities tied to a location ---
    public async Task<List<ActivityDto>> GetActivitiesByLocationIdAsync(int locationId)
    {
        var location = await _locationRepo.GetByIdAsync(locationId);
        if (location == null)
            return new List<ActivityDto>();

        // - If the location exists, fetch all activities and participants to calculate current participant counts
        var allActivities = await _activityRepo.GetAllAsync();
        var allParticipants = await _participantRepo.GetAllAsync();

        // - Filter activities by the specified location and map them to ActivityDto, including current participant counts and full status
        return allActivities
            .Where(a => a.LocationId == locationId)
            .Select(a =>
            {
                // - Calculate the current number of participants for this activity
                var currentCount = allParticipants.Count(p => p.ActivityId == a.Id);

                // - Create and return the ActivityDto with all relevant information sorted by scheduled time
                return new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    ScheduledAt = a.ScheduledAt,
                    MaxParticipants = a.MaxParticipants,
                    CurrentParticipants = currentCount,
                    IsFull = currentCount >= a.MaxParticipants,
                    LocationId = location.Id,
                    LocationName = location.Name,
                    CreatedByUserId = a.CreatedByUserId
                };
            })
            .OrderBy(a => a.ScheduledAt)
            .ToList();
    }

    // 3. Join an existing activity ---
    public async Task JoinActivityAsync(string userId, int activityId)
    {
        // - Fetch the activity by its ID and ensure it exists
        var activity = await _activityRepo.GetByIdAsync(activityId);
        if (activity == null)
            throw new KeyNotFoundException("Activity not found");

        // - Check if the user has passed the quiz for the location associated with this activity
        var hasPassed = await _quizService.HasPassedQuizAsync(userId, activity.LocationId);
        if (!hasPassed)
            throw new InvalidOperationException("You must pass this location's quiz (75%+) before joining an activity here");

        // - Fetch all participants to check if the user has already joined and to count current participants
        var allParticipants = await _participantRepo.GetAllAsync();
        var existing = allParticipants.Any(p => p.ActivityId == activityId && p.UserId == userId);
        if (existing)
            throw new InvalidOperationException("You have already joined this activity");

        var currentCount = allParticipants.Count(p => p.ActivityId == activityId);
        if (currentCount >= activity.MaxParticipants)
            throw new InvalidOperationException("This activity is full");

        // - Create a new ActivityParticipant and save it to the database to represent the user joining the activity
        var participant = new ActivityParticipant
        {
            ActivityId = activityId,
            UserId = userId
        };

        await _participantRepo.AddAsync(participant);
        await _participantRepo.SaveChangesAsync();
    }



    // Full detail view of a single activity (creator name + participant names) ---
    public async Task<ActivityDetailDto?> GetActivityByIdAsync(int activityId)
    {
        var activity = await _activityRepo.GetByIdAsync(activityId);
        if (activity == null)
            return null;

        var location = await _locationRepo.GetByIdAsync(activity.LocationId);

        var allUsers = await _userRepo.GetAllAsync();
        var allParticipants = await _participantRepo.GetAllAsync();

        var creator = allUsers.FirstOrDefault(u => u.Id == activity.CreatedByUserId);

        var participantUserIds = allParticipants
            .Where(p => p.ActivityId == activityId)
            .Select(p => p.UserId)
            .ToList();

        var participantNames = allUsers
            .Where(u => participantUserIds.Contains(u.Id))
            .Select(u => u.FullName ?? u.Email ?? "Unknown user")
            .ToList();

        return new ActivityDetailDto
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            ScheduledAt = activity.ScheduledAt,
            MaxParticipants = activity.MaxParticipants,
            CurrentParticipants = participantNames.Count,
            IsFull = participantNames.Count >= activity.MaxParticipants,
            LocationId = activity.LocationId,
            LocationName = location?.Name ?? "Unknown location",
            CreatedByUserId = activity.CreatedByUserId,
            CreatedByName = creator?.FullName ?? creator?.Email ?? "Unknown user",
            Participants = participantNames
        };
    }
}