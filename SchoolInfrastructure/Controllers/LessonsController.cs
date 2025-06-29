using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolDomain.Model;

namespace SchoolInfrastructure.Controllers
{
    public class LessonsController : Controller
    {
        private readonly AsplabContext _context;

        public LessonsController(AsplabContext context)
        {
            _context = context;
        }

        // GET: Lessons
        public async Task<IActionResult> Index(int? id, string name)
        {
            // Якщо id не передано, можна або показати всі уроки, або повернути помилку
            if (!id.HasValue)
            {
                // Виберіть потрібну поведінку:
                // 1. Показати всі уроки:
                var allLessons = _context.Lessons
                    .Include(l => l.Course)
                    .Include(l => l.Teacher);
                ViewData["CourseName"] = "Всі курси";
                return View(await allLessons.ToListAsync());

                // 2. Або повернути помилку:
                // return NotFound("Курс не вказано.");
            }

            // Фільтруємо уроки за CourseId
            var lessons = _context.Lessons
                .Where(l => l.CourseId == id)
                .Include(l => l.Course)
                .Include(l => l.Teacher);

            // Передаємо назву курсу у ViewData для відображення
            ViewData["CourseName"] = name ?? (await _context.Courses
                .Where(c => c.Id == id)
                .Select(c => c.Name)
                .FirstOrDefaultAsync()) ?? "Невідомий курс";

            ViewData["CourseId"] = id; // Для використання в представленні, наприклад, для створення нових уроків

            return View(await lessons.ToListAsync());
        }

        public IActionResult DetailsPartial(int id)
        {
            var lesson = _context.Lessons
                .Include(l => l.Teacher)
                .ThenInclude(t => t.User)
                .Include(l => l.Course)
                .FirstOrDefault(l => l.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            return PartialView("_LessonDetailsPartial", lesson);
        }


        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Lessons/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeacherId,CourseId,Description,Name,LessonDate")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", lesson.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id", lesson.TeacherId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", lesson.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id", lesson.TeacherId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,CourseId,Description,Name,LessonDate")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", lesson.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id", lesson.TeacherId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
