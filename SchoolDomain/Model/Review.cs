using System;
using System.Collections.Generic;

namespace SchoolDomain.Model;

public partial class Review
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public int? Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
