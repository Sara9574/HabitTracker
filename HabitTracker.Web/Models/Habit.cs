namespace HabitTracker.Web.Models
{
    public class Habit
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int WeeklyGoal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<HabitCompletion> Completions { get; set; } = new();
    }
}