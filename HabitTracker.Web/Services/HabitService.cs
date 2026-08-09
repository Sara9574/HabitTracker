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

        public async Task<List<Habit>> GetHabitsAsync(string userId)
        {
            return await _context.Habits
                .Where(h => h.UserId == userId)
                .Include(h => h.Completions)
                .ToListAsync();
        }

        public async Task<Habit?> GetHabitByIdAsync(int id, string userId)
        {
            return await _context.Habits
                .Include(h => h.Completions)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        }

        public async Task AddHabitAsync(Habit habit, string userId)
        {
            habit.UserId = userId;
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
        }

        public async Task EditHabitAsync(int id, Habit habit, string userId)
        {
            var existingHabit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (existingHabit != null)
            {
                existingHabit.Name = habit.Name;
                existingHabit.WeeklyGoal = habit.WeeklyGoal;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveHabitAsync(int id, string userId)
        {
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (habit != null)
            {
                _context.Habits.Remove(habit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleCompletionAsync(int habitId, string userId)
        {
            var habit = await _context.Habits
                .Include(h => h.Completions)
                .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

            if (habit == null)
                return; // not found, or doesn't belong to this user

            var today = DateTime.UtcNow.Date;
            var existing = habit.Completions.FirstOrDefault(c => c.Date.Date == today);

            if (existing != null)
            {
                // Already marked today -> un-mark it
                habit.Completions.Remove(existing);
                _context.HabitCompletions.Remove(existing);
            }
            else
            {
                // Not marked yet today -> mark it
                habit.Completions.Add(new HabitCompletion
                {
                    HabitId = habitId,
                    Date = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        public bool IsCompletedToday(Habit habit)
        {
            var today = DateTime.UtcNow.Date;
            return habit.Completions.Any(c => c.Date.Date == today);
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

        public int GetCurrentStreak(Habit habit)
        {
            var dates = habit.Completions
                .Select(c => c.Date.Date)
                .Distinct()
                .ToHashSet();

            if (dates.Count == 0)
                return 0;

            var today = DateTime.UtcNow.Date;
            var cursor = dates.Contains(today) ? today : today.AddDays(-1);

            if (!dates.Contains(cursor))
                return 0;

            int streak = 0;
            while (dates.Contains(cursor))
            {
                streak++;
                cursor = cursor.AddDays(-1);
            }

            return streak;
        }

        public int GetLongestStreak(Habit habit)
        {
            var dates = habit.Completions
                .Select(c => c.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (dates.Count == 0)
                return 0;

            int longest = 1;
            int current = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                var gap = (dates[i] - dates[i - 1]).Days;

                if (gap == 1)
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else
                {
                    current = 1;
                }
            }

            return longest;
        }
    }
}
