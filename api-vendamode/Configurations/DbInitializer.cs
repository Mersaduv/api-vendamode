using api_vendace.Entities.Users.Security;

namespace api_vendace.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Roles.Any(r => r.Title == "مشتری"))
        {
            var customerRole = new Role { Title = "مشتری", IsActive = true };
            context.Roles.Add(customerRole);
            context.SaveChanges();
        }
    }
}