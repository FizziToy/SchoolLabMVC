using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolInfrastructure.Data;
using Microsoft.AspNetCore.Identity;
using SchoolDomain.Model;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;

namespace SchoolInfrastructure.Controllers
{
    public class TeachersController : Controller
    {
        private readonly AsplabContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TeachersController(AsplabContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.UserId != null)
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            var isStudent = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Student");
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");
            var currentStudent = currentUser != null ? await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.Id) : null;

            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .ThenInclude(s => s.User)
                .ToListAsync();

            // Передаємо дані через ViewData
            ViewData["IsStudent"] = isStudent;
            ViewData["IsAdmin"] = isAdmin;
            ViewData["CurrentUserId"] = currentUser?.Id;
            ViewData["CurrentStudent"] = currentStudent;
            ViewData["Reviews"] = reviews;

            return View(teachers);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Content("Помилка: Користувач не авторизований.");
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == currentUser.Id);

            if (teacher == null)
            {
                return Content("Помилка: Профіль вчителя не знайдено для UserId: " + currentUser.Id);
            }

            return View(teacher);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher, IFormFile TeacherPhoto)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var existingTeacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == currentUser.Id);

            if (existingTeacher == null)
            {
                return NotFound("Профіль вчителя не знайдено.");
            }

            if (existingTeacher.UserId != currentUser.Id)
            {
                return Forbid("Ви можете редагувати лише свій профіль вчителя.");
            }

            // Оновлення полів
            existingTeacher.Description = teacher.Description;
            existingTeacher.LessonLink = teacher.LessonLink;

            // Обробка завантаження фото
            if (TeacherPhoto != null && TeacherPhoto.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(TeacherPhoto.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("TeacherPhoto", "Дозволені лише файли .jpg, .jpeg, .png, .gif.");
                    return View(teacher);
                }

                // Читання зображення
                using (var stream = TeacherPhoto.OpenReadStream())
                using (var image = System.Drawing.Image.FromStream(stream))
                {
                    // Обчислення нових розмірів (ширина = 250 пікселів, пропорційна висота)
                    int newWidth = 250;
                    int newHeight = (int)(image.Height * ((float)newWidth / image.Width));

                    // Зміна розміру зображення
                    using (var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight))
                    using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                    {
                        graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                        // Збереження зображення
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/teachers", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        resizedImage.Save(filePath, image.RawFormat);
                        existingTeacher.TeacherUrl = $"/images/teachers/{fileName}";
                    }
                }
            }

            try
            {
                _context.Update(existingTeacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Не вдалося зберегти зміни. Спробуйте ще раз.");
                return View(teacher);
            }
        }
    }
}