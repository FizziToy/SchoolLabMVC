using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class Teacher
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? UserId { get; set; }

    public string? TeacherUrl { get; set; }

    public string? LessonLink { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual AspNetUser? User { get; set; }
}
