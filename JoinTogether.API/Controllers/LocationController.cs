using JoinTogether.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationSummaryDto>>> GetAll()
    {
        var locations = await _locationService.GetAllLocationsAsync();

        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDetailDto>> GetById(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);

        if (location == null)
            return NotFound();

        return Ok(location);
    }
}