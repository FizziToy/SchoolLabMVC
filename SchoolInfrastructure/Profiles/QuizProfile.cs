using AutoMapper;
using SchoolDomain.Model;
using SchoolInfrastructure.Models.Dtos;
using SchoolInfrastructure.Models.ViewModels;

namespace SchoolInfrastructure.Profiles
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<Quiz, QuizDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));
            CreateMap<Question, QuestionDto>();
            CreateMap<Course, CourseDto>();
            CreateMap<CreateQuizViewModel, Quiz>();
            CreateMap<CreateQuestionViewModel, Question>();
        }
    }
}
