using HabitTracker.Web.Data;
using HabitTracker.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Web.Services
{
    public class HabitService : IHabitService
    {
        private readonly AppDbContext _context;

        public HabitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Habit>> GetHabitsAsync()
        {
            return await _context.Habits
                .Include(h => h.Completions)
                .ToListAsync();
        }

        public async Task<Habit?> GetHabitByIdAsync(int id)
        {
            return await _context.Habits
                .Include(h => h.Completions)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task AddHabitAsync(Habit habit)
        {
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
        }

        public async Task EditHabitAsync(int id, Habit habit)
        {
            var existingHabit = await _context.Habits.FindAsync(id);
            if (existingHabit != null)
            {
                existingHabit.Name = habit.Name;
                existingHabit.WeeklyGoal = habit.WeeklyGoal;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveHabitAsync(int id)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCompletionAsync(int habitId)
        {
            var habit = await _context.Habits
                .Include(h => h.Completions)
                .FirstOrDefaultAsync(h => h.Id == habitId);

            if (habit == null)
                return;

            habit.Completions.Add(new HabitCompletion
            {
                HabitId = habitId,
                Date = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        public int GetWeeklyCompletions(Habit habit)
        {
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            return habit.Completions.Count(c => c.Date >= startOfWeek);
        }

        public int GetProgressPercentage(Habit habit)
        {
            var weekly = GetWeeklyCompletions(habit);

            if (habit.WeeklyGoal == 0)
                return 0;

            return (int)((double)weekly / habit.WeeklyGoal * 100);
        }
        public string GetProgressColor(int percentage)
        {
            if (percentage >= 100)
                return "bg-success";   // green
            if (percentage >= 50)
                return "bg-warning";   // yellow
            return "bg-danger";        // red
        }


        public Dictionary<int, string> GetHabitColors(List<Habit> habits)
        {
            string[] colors = new[]
            {
        "#4caf50", // green
        "#2196f3", // blue
        "#9c27b0", // purple
        "#ff9800", // orange
        "#e91e63", // pink
        "#795548", // brown
        "#009688", // teal
        "#3f51b5", // indigo
    };

            var map = new Dictionary<int, string>();

            for (int i = 0; i < habits.Count; i++)
            {
                map[habits[i].Id] = colors[i % colors.Length];
            }

            return map;
        }

    }
}
