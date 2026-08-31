namespace JoinTogether.Shared.DTOs;

// Used for the "browse locations" list view — lightweight, no quiz question details
public class LocationSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Category { get; set; }
    public int QuizQuestionCount { get; set; }
}

// Used when the user opens a single location (still no correct-answer info)
public class LocationDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Category { get; set; }

    // Number of quiz questions for this location (but not the questions themselves)
    public int QuizQuestionCount { get; set; }
}