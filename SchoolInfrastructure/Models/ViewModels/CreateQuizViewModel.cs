using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SchoolInfrastructure.Models.ViewModels
{
    public class CreateQuizViewModel
    {
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Виберіть курс для тесту.")]
    public int CourseId { get; set; }

    public List<SelectListItem> AvailableCourses { get; set; } = new List<SelectListItem>();

    [Required]
    [MinLength(1, ErrorMessage = "Необхідно додати хоча б одне питання.")]
    public List<CreateQuestionViewModel> Questions { get; set; } = new List<CreateQuestionViewModel>();
     }
}
