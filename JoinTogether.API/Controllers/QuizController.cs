using JoinTogether.BLL.Interfaces;
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
}