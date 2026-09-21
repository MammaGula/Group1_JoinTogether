using JoinTogether.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JoinTogether.Shared.DTOs;

namespace JoinTogether.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    // 1. GET api/Activity/location/5
    // View all activities tied to a location
    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetByLocationId(int locationId)
    {
        var activities = await _activityService.GetActivitiesByLocationIdAsync(locationId);
        return Ok(activities);
    }

    // 2. POST api/Activity
    // Create an activity (requires having passed the location's quiz)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto request)
    {
        // - User must be authenticated to create an activity
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User is not authenticated" });

        // - Call the service to create the activity and handle exceptions
        try
        {
            var activity = await _activityService.CreateActivityAsync(userId, request);
            return Ok(activity);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 3. POST api/Activity/5/join
    // Join an existing activity
    [Authorize]
    [HttpPost("{activityId}/join")]
    public async Task<IActionResult> JoinActivity(int activityId)
    {
        // - User must be authenticated to join an activity
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User is not authenticated" });

        // - Call the service to join the activity and handle exceptions
        try
        {
            await _activityService.JoinActivityAsync(userId, activityId);
            return Ok(new { message = "Joined activity successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
