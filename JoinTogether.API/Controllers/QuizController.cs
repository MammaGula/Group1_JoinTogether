using JoinTogether.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JoinTogether.Shared.DTOs;

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
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User is not authenticated" });

        var result = await _quizService.SubmitQuizAsync(userId, request);
        return Ok(result);
    }
}