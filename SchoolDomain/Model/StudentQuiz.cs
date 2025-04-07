using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class StudentQuiz
{
    public int? StudentCourseId { get; set; }

    public int? QuizId { get; set; }

    public int Score { get; set; }

    public DateOnly? QuizDate { get; set; }

    public virtual Quiz? Quiz { get; set; }

    public virtual StudentCourse? StudentCourse { get; set; }
}
