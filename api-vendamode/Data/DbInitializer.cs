using api_vendamode.Entities.Users.Security;

namespace api_vendamode.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Roles.Any(r => r.Title == "customer"))
        {
            var customerRole = new Role { Title = "customer", IsActive = true };
            context.Roles.Add(customerRole);
            context.SaveChanges();
        }
    }
}