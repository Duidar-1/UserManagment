using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<RolesController> _localizer;

        public RolesController(IRoleService roleService, IStringLocalizer<RolesController> localizer)
        {
            _roleService = roleService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(new { Message = _localizer["RoleNotFound"].Value });
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdRole = await _roleService.CreateRoleAsync(role);
                return CreatedAtAction(nameof(GetRole), new { id = createdRole.Id }, createdRole);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { Message = _localizer["RoleNameExists"].Value });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role role)
        {
            if (id != role.Id)
                return BadRequest(new { Message = _localizer["RoleIdMismatch"].Value });

            try
            {
                await _roleService.UpdateRoleAsync(role);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = _localizer["RoleNotFound"].Value });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = _localizer["RoleNotFound"].Value });
            }
        }
    }
}
