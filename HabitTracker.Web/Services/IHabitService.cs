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
        Task AddCompletionAsync(int habitId);
        int GetWeeklyCompletions(Habit habit);
        int GetProgressPercentage(Habit habit);
        string GetProgressColor(int percentage);
    }
}
