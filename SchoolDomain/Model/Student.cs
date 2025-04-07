using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class Student
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public int? TotalCourses { get; set; }

    public int? CompletedCourses { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual AspNetUser? User { get; set; }
}
