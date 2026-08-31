using JoinTogether.BLL.Interfaces;
using JoinTogether.DAL.Repositories;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    // --- Browse all locations ---
    public async Task<List<LocationSummaryDto>> GetAllLocationsAsync()
    {
        var locations = await _locationRepository.GetAllAsync();

        return locations.Select(l => new LocationSummaryDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Category = l.Category,
            QuizQuestionCount = l.QuizQuestions?.Count ?? 0
        }).ToList();
    }

    // --- View a single location ---
    public async Task<LocationDetailDto?> GetLocationByIdAsync(int locationId)
    {
        var location = await _locationRepository.GetWithQuizAsync(locationId);
        if (location == null)
            return null;

        return new LocationDetailDto
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Category = location.Category,
            QuizQuestionCount = location.QuizQuestions.Count
        };
    }
}