using JoinTogether.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JoinTogether.DAL.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivityParticipant> ActivityParticipants => Set<ActivityParticipant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<QuizQuestion>()
            .HasOne(q => q.Location)
            .WithMany(l => l.QuizQuestions)
            .HasForeignKey(q => q.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuizOption>()
            .HasOne(o => o.QuizQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuizAttempt>()
            .HasOne(a => a.Location)
            .WithMany(l => l.QuizAttempts)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<QuizAttempt>()
            .HasOne(a => a.User)
            .WithMany(u => u.QuizAttempts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Activity>()
            .HasOne(a => a.Location)
            .WithMany(l => l.Activities)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Activity>()
            .HasOne(a => a.CreatedBy)
            .WithMany(u => u.CreatedActivities)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ActivityParticipant>()
            .HasOne(p => p.Activity)
            .WithMany(a => a.Participants)
            .HasForeignKey(p => p.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ActivityParticipant>()
            .HasOne(p => p.User)
            .WithMany(u => u.ActivityParticipations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ActivityParticipant>()
            .HasIndex(p => new { p.ActivityId, p.UserId })
            .IsUnique();
    }
}