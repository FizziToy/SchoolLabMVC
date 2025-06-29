using SchoolInfrastructure.Models.Dtos;

namespace SchoolInfrastructure.Models.ViewModels
{
    public class QuizViewModel
    {
        public QuizDto Quiz { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
