using Microsoft.AspNetCore.Identity;

namespace HabitTracker.Web.Models
{
    // Extends IdentityUser so we have room to add profile fields later
    // (display name, timezone, etc.) without another migration overhaul.
    public class ApplicationUser : IdentityUser
    {
    }
}
