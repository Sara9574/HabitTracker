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
            var completion = new HabitCompletion
            {
                HabitId = habitId,
                Date = DateTime.UtcNow
            };

            _context.HabitCompletions.Add(completion);
            await _context.SaveChangesAsync();
        }
    }
}
