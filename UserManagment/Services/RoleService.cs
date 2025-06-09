using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            using var connection = _context.Database.GetDbConnection() as SqlConnection;
            await connection.OpenAsync();
            var roles = new List<Role>();

            using var command = new SqlCommand("SELECT Id, Name, Description FROM Roles", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }

            return roles;
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateRoleAsync(Role role)
        {
            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole == null)
                throw new KeyNotFoundException("Role not found");

            existingRole.Name = role.Name;
            existingRole.Description = role.Description;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

}
