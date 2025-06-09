using UserManagement.Models;

namespace UserManagement.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure the database is created and migrated
            context.Database.EnsureCreated();

            // Check if roles already exist
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role
                    {
                        Name = "Admin",
                        Description = "Administrator role with full access"
                    },
                    new Role
                    {
                        Name = "User",
                        Description = "Standard user role"
                    }
                );
                context.SaveChanges();
            }

            // Check if users already exist
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");

                if (adminRole != null && userRole != null)
                {
                    context.Users.AddRange(
                        new User
                        {
                            Username = "admin",
                            Email = "admin@gmail.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                            Roles = new List<Role> { adminRole }
                        },
                        new User
                        {
                            Username = "user",
                            Email = "user@gmail.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                            Roles = new List<Role> { userRole }
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }

}
