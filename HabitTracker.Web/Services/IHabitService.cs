using HabitTracker.Web.Models;

namespace HabitTracker.Web.Services
{
    public interface IHabitService
    {
        Task<List<Habit>> GetHabitsAsync(string userId);
        Task<Habit?> GetHabitByIdAsync(int id, string userId);
        Task AddHabitAsync(Habit habit, string userId);
        Task EditHabitAsync(int id, Habit habit, string userId);
        Task RemoveHabitAsync(int id, string userId);
        Task ToggleCompletionAsync(int habitId, string userId);
        bool IsCompletedToday(Habit habit);
        int GetWeeklyCompletions(Habit habit);
        int GetProgressPercentage(Habit habit);
        string GetProgressColor(int percentage);
        Dictionary<int, string> GetHabitColors(List<Habit> habits);
        int GetCurrentStreak(Habit habit);
        int GetLongestStreak(Habit habit);
    }
}
