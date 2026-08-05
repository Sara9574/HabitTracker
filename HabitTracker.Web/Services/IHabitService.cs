using HabitTracker.Web.Models;

namespace HabitTracker.Web.Services
{
    public interface IHabitService
    {
        Task<List<Habit>> GetHabitsAsync();
        Task<Habit?> GetHabitByIdAsync(int id);
        Task AddHabitAsync(Habit habit);
        Task EditHabitAsync(int id, Habit habit);
        Task RemoveHabitAsync(int id);
        Task ToggleCompletionAsync(int habitId);
        bool IsCompletedToday(Habit habit);
        int GetWeeklyCompletions(Habit habit);
        int GetCurrentStreak(Habit habit);
        int GetLongestStreak(Habit habit);
        Dictionary<DayOfWeek, int> GetCompletionsByDayOfWeek(List<Habit> habits);
        int GetProgressPercentage(Habit habit);
        string GetProgressColor(int percentage);
        Dictionary<int, string> GetHabitColors(List<Habit> habits);
    }
}