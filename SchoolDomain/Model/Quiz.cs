using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class Quiz
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public DateOnly? Created { get; set; }

    public int? CourseId { get; set; }

    public string? Description { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
