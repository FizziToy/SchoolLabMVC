using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDomain.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolInfrastructure.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AsplabContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(AsplabContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews/Create?teacherId={id}
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> Create(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                return NotFound("Вчителя не знайдено.");
            }

            var model = new Review { TeacherId = teacherId };
            return View(model);
        }

        // POST: Reviews/Create
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (!ModelState.IsValid)
            {
                // Додаємо повідомлення про помилки валідації
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Помилка валідації: " + string.Join("; ", errors);
                return View(review);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["Error"] = "Користувач не авторизований.";
                return Unauthorized();
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.Id);
            if (student == null)
            {
                TempData["Error"] = "Ви не є студентом.";
                return Forbid("Ви не є студентом.");
            }

            review.StudentId = student.Id;
            review.CreatedAt = DateTime.UtcNow;
            review.Date = DateOnly.FromDateTime(DateTime.Today);

            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Відгук успішно створено.";
                return RedirectToAction("Index", "Teachers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при збереженні відгуку: {ex.Message}";
                return View(review);
            }
        }

        // GET: Reviews/Edit/{id}
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                TempData["Error"] = "Відгук не знайдено.";
                return NotFound("Відгук не знайдено.");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.Id);
            if (student == null || review.StudentId != student.Id)
            {
                TempData["Error"] = "Ви можете редагувати лише свої відгуки.";
                return Forbid("Ви можете редагувати лише свої відгуки.");
            }

            if (review.CreatedAt.AddMinutes(5) < DateTime.UtcNow)
            {
                TempData["Error"] = "Редагування відгуку можливо лише протягом 5 хвилин після створення.";
                return Forbid("Редагування відгуку можливо лише протягом 5 хвилин після створення.");
            }

            return View(review);
        }

        // POST: Reviews/Edit/{id}
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            if (id != review.Id)
            {
                TempData["Error"] = "Невідповідність ID відгуку.";
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Помилка валідації: " + string.Join("; ", errors);
                return View(review);
            }

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                TempData["Error"] = "Відгук не знайдено.";
                return NotFound("Відгук не знайдено.");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.Id);
            if (student == null || existingReview.StudentId != student.Id)
            {
                TempData["Error"] = "Ви можете редагувати лише свої відгуки.";
                return Forbid("Ви можете редагувати лише свої відгуки.");
            }

            if (existingReview.CreatedAt.AddMinutes(5) < DateTime.UtcNow)
            {
                TempData["Error"] = "Редагування відгуку можливо лише протягом 5 хвилин після створення.";
                return Forbid("Редагування відгуку можливо лише протягом 5 хвилин після створення.");
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;

            try
            {
                _context.Update(existingReview);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Відгук успішно оновлено.";
                return RedirectToAction("Index", "Teachers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при оновленні відгуку: {ex.Message}";
                return View(review);
            }
        }

        // POST: Reviews/Delete/{id}
        [Authorize(Roles = "Student,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                TempData["Error"] = "Відгук не знайдено.";
                return NotFound("Відгук не знайдено.");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUser.Id);

            if (!isAdmin && (student == null || review.StudentId != student.Id))
            {
                TempData["Error"] = "Ви можете видаляти лише свої відгуки.";
                return Forbid("Ви можете видаляти лише свої відгуки.");
            }

            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Відгук успішно видалено.";
                return RedirectToAction("Index", "Teachers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні відгуку: {ex.Message}";
                return RedirectToAction("Index", "Teachers");
            }
        }
    }
}