using System.ComponentModel.DataAnnotations;
namespace HabitTracker.Web.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; } = string.Empty;

        [Range(1, 100)]
        public int WeeklyGoal { get; set; }

        public DateTime CreatedAt { get; set; }

        // Ownership: every habit belongs to exactly one user.
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public List<HabitCompletion> Completions { get; set; } = new();
    }
}