using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDomain.Model;
using SchoolInfrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolInfrastructure.Controllers
{
    public class ChartsController : Controller
    {
        private readonly AsplabContext _context;

        public ChartsController(AsplabContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courseCount = await _context.Courses.CountAsync();
            var lessonCount = await _context.Lessons.CountAsync();
            Console.WriteLine($"Courses count: {courseCount}, Lessons count: {lessonCount}");

            var coursesData = await _context.Courses
                .Where(c => c.Price.HasValue && c.Price > 0)
                .GroupBy(c => c.Price)
                .Select(g => new { price = g.Key, count = g.Count() })
                .ToListAsync();

            var lessonsData = await _context.Lessons
                .Include(l => l.Course)
                .GroupBy(l => l.Course != null ? l.Course.Name ?? "Без назви" : "Без курсу")
                .Select(g => new { course = g.Key, count = g.Count() })
                .ToListAsync();

            Console.WriteLine($"Courses data count: {coursesData.Count}");
            foreach (var item in coursesData)
            {
                Console.WriteLine($"Price: {item.price}, Count: {item.count}");
            }
            Console.WriteLine($"Lessons data count: {lessonsData.Count}");
            foreach (var item in lessonsData)
            {
                Console.WriteLine($"Course: {item.course}, Count: {item.count}");
            }

            ViewBag.CoursesData = coursesData;
            ViewBag.LessonsData = lessonsData;
            ViewBag.CourseCount = courseCount;
            ViewBag.LessonCount = lessonCount;
            ViewBag.RawCoursesData = System.Text.Json.JsonSerializer.Serialize(coursesData);
            ViewBag.RawLessonsData = System.Text.Json.JsonSerializer.Serialize(lessonsData);

            return View();
        }
    }
}