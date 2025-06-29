using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolDomain.Model;
using Microsoft.EntityFrameworkCore;
using SchoolInfrastructure.Data;

namespace SchoolInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AsplabContext _context;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AsplabContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Список користувачів із фільтрацією
        public async Task<IActionResult> Index(string? search, string? role)
        {
            var users = _userManager.Users.AsQueryable();

            // Пошук за email
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                users = users.Where(u => u.Email.ToLower().Contains(search));
            }

            // Фільтрація за роллю
            var userRoles = new Dictionary<string, IList<string>>();
            var filteredUsers = new List<IdentityUser>();
            foreach (var user in users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles;
                if (string.IsNullOrEmpty(role) || roles.Contains(role))
                {
                    filteredUsers.Add(user);
                }
            }

            // Доступні ролі для фільтра
            var availableRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewData["UserRoles"] = userRoles;
            ViewData["SearchQuery"] = search;
            ViewData["SelectedRole"] = role;
            ViewData["AvailableRoles"] = availableRoles;

            return View(filteredUsers);
        }

        // Оновлення ролей користувача
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Користувач ще не підтвердив email.");
                return RedirectToAction("Index");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).Where(r => r != "Admin").ToList();

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не вдалося додати ролі.");
                return RedirectToAction("Index");
            }

            result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не вдалося видалити ролі.");
                return RedirectToAction("Index");
            }
            if (rolesToAdd.Contains("Teacher") && !await _context.Teachers.AnyAsync(t => t.UserId == userId))
            {
                var teacher = new Teacher { UserId = userId };
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
            }
            if (rolesToAdd.Contains("Student") && !await _context.Students.AnyAsync(s => s.UserId == userId))
            {
                var student = new Student { UserId = userId };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
            if (rolesToRemove.Contains("Teacher"))
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher != null)
                {
                    _context.Teachers.Remove(teacher);
                    await _context.SaveChangesAsync();
                }
            }
            if (rolesToRemove.Contains("Student"))
            {
                var student = await _context.Students.FirstOrDefaultAsync(t => t.UserId == userId);
                if (student != null)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // Видалення користувача
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Захист від видалення адміна
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                ModelState.AddModelError("", "Неможливо видалити адміністратора.");
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не вдалося видалити користувача.");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}