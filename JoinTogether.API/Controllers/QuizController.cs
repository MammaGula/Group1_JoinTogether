using System.Security.Claims;
using JoinTogether.BLL.Interfaces;
using JoinTogether.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoinTogether.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    // GET api/Quiz/location/5
    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetByLocationId(int locationId)
    {
        var quiz = await _quizService.GetQuizByLocationIdAsync(locationId);

        // Handle missing/invalid location data
        if (quiz == null)
            return NotFound(new { message = $"No location found for id {locationId}" });

        if (quiz.Questions.Count == 0)
            return NotFound(new { message = "This location has no quiz questions yet" });

        return Ok(quiz);
    }

    // POST api/Quiz/submit
    // Requires a logged-in user (JWT) so the attempt can be tied to their account.
    [HttpPost("submit")]
    [Authorize]
    public async Task<IActionResult> Submit(SubmitQuizRequest request)
    {
        // The user id was put into the JWT as ClaimTypes.NameIdentifier at login (see AuthService.CreateToken)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _quizService.SubmitQuizAsync(userId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            // Thrown by QuizService when request.LocationId doesn't exist
            return NotFound(new { message = ex.Message });
        }
    }
}