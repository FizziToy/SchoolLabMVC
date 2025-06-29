using System.ComponentModel.DataAnnotations;
namespace SchoolInfrastructure.Models.ViewModels
{
    public class CreateQuestionViewModel
    {
        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string AnswerA { get; set; }

        [Required]
        public string AnswerB { get; set; }

        [Required]
        public string AnswerC { get; set; }

        [Required]
        public string AnswerD { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }
    }
}
