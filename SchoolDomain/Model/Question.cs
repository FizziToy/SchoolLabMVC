using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class Question
{
    public int Id { get; set; }

    public int? QuizId { get; set; }

    public string? QuestionText { get; set; }

    public string? AnswerA { get; set; }

    public string? AnswerB { get; set; }

    public string? AnswerC { get; set; }

    public string? AnswerD { get; set; }

    public string? CorrectAnswer { get; set; }

    public virtual Quiz? Quiz { get; set; }
}
