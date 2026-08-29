using JoinTogether.DAL.Entities;

namespace JoinTogether.DAL.Data;

/// <summary>
/// - Seed data for the PBIs "Learn about Malmö City Library" and "Learn about Turning Torso".
/// - Matches the existing entities in the repository: Location -> QuizQuestion -> QuizOption.
/// - Each Location gets 4 quiz questions (per the MVP example in the project idea: 3/4 correct = 75% = pass).
///
/// - Call this once, for example immediately after the database has been migrated in Program.cs:
///
///     using (var scope = app.Services.CreateScope())
///     {
///         var db = scope.ServiceProvider.GetRequiredService&lt;AppDbContext&gt;();
///         db.Database.Migrate();
///         LocationQuizSeeder.Seed(db);
///     }
/// </summary>
public static class LocationQuizSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Prevent seeding if the database already has locations
        if (context.Locations.Any())
        {
            return; // already seeded, do not run again
        }

        var library = new Location
        {
            Name = "Malmö City Library",
            Description =
                "Malmö City Library is located in the southeastern corner of Slottsparken and opened to the public " +
                "in 1905. Today the library consists of three interconnected buildings: the historic \"The Castle\" " +
                "(designed by John Smedberg and Fredrik Sundbärg, and used as the main library since 1946), the modern " +
                "glass building \"Calendar of Light\" (designed by the Danish architect Henning Larsen and inaugurated in 1997) " +
                "and \"The Cylinder\" which connects the entrance, reception, and café. In 1997 the library was awarded " +
                "the Kasper Salin Prize for its architecture.",
            Latitude = 55.6046,
            Longitude = 13.0016,
            Category = "Culture",
            QuizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "In which year did Malmö City Library first open to the public?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "1901", IsCorrect = false },
                        new QuizOption { Text = "1905", IsCorrect = true },
                        new QuizOption { Text = "1927", IsCorrect = false },
                        new QuizOption { Text = "1997", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "Who designed the modern glass building \"Calendar of Light\"?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "Santiago Calatrava", IsCorrect = false },
                        new QuizOption { Text = "Henning Larsen", IsCorrect = true },
                        new QuizOption { Text = "Gunnar Asplund", IsCorrect = false },
                        new QuizOption { Text = "John Smedberg", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "Which three buildings make up the library today?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "The Castle, The Cylinder, and Calendar of Light", IsCorrect = true },
                        new QuizOption { Text = "The Castle, The Tower, and The Bazaar", IsCorrect = false },
                        new QuizOption { Text = "The Cube, The Cylinder, and The Pyramid", IsCorrect = false },
                        new QuizOption { Text = "Calendar of Light, The Museum, and The Archive", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "Which architecture prize did the library win in 1997?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "The Kasper Salin Prize", IsCorrect = true },
                        new QuizOption { Text = "Design S", IsCorrect = false },
                        new QuizOption { Text = "Guldbaggen", IsCorrect = false },
                        new QuizOption { Text = "The Nordic Council Prize", IsCorrect = false },
                    }
                },
            }
        };

        var turningTorso = new Location
        {
            Name = "Turning Torso",
            Description =
                "Turning Torso in the Western Harbour is the tallest residential building in the Nordic region and one of Malmö's most prominent landmarks. " +
                "The tower is approximately 190 meters tall, has 54 floors divided across nine stacked \"cubes\", and twists a total of 90 degrees from bottom to top. " +
                "The building was designed by the Spanish-Swiss architect, artist, and engineer Santiago Calatrava, inspired by a human body in a twisting motion. " +
                "Turning Torso was completed and inaugurated in 2005 and became a symbol of the transformation of the Western Harbour from an industrial area into a modern district.",
            Latitude = 55.6135,
            Longitude = 12.9761,
            Category = "Architecture",
            QuizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "Who is the architect behind Turning Torso?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "Santiago Calatrava", IsCorrect = true },
                        new QuizOption { Text = "Henning Larsen", IsCorrect = false },
                        new QuizOption { Text = "Zaha Hadid", IsCorrect = false },
                        new QuizOption { Text = "Gunnar Asplund", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "How many floors does Turning Torso have in total?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "34", IsCorrect = false },
                        new QuizOption { Text = "44", IsCorrect = false },
                        new QuizOption { Text = "54", IsCorrect = true },
                        new QuizOption { Text = "64", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "In which year was Turning Torso inaugurated?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "1997", IsCorrect = false },
                        new QuizOption { Text = "2001", IsCorrect = false },
                        new QuizOption { Text = "2005", IsCorrect = true },
                        new QuizOption { Text = "2010", IsCorrect = false },
                    }
                },
                new QuizQuestion
                {
                    QuestionText = "How many degrees does the building twist from bottom to top?",
                    Options = new List<QuizOption>
                    {
                        new QuizOption { Text = "45 degrees", IsCorrect = false },
                        new QuizOption { Text = "90 degrees", IsCorrect = true },
                        new QuizOption { Text = "180 degrees", IsCorrect = false },
                        new QuizOption { Text = "360 degrees", IsCorrect = false },
                    }
                },
            }
        };

        // Add the locations to the context and save changes
        context.Locations.AddRange(library, turningTorso);
        context.SaveChanges();
    }
}