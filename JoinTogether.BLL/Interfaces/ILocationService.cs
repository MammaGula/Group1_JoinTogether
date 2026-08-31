using JoinTogether.Shared.DTOs;

namespace JoinTogether.BLL.Interfaces;

public interface ILocationService
{
    // US-01: browse all locations (with quiz info)
    Task<List<LocationSummaryDto>> GetAllLocationsAsync();

    // US-01: view a single location's details
    Task<LocationDetailDto?> GetLocationByIdAsync(int locationId);
}