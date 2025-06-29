using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolDomain.Model;

public partial class Review
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }
    [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5.")]
    public int? Rating { get; set; }
    [Required(ErrorMessage = "Коментар є обов'язковим.")]
    public string Comment { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Student? Student { get; set; } 

    public virtual Teacher? Teacher { get; set; } 
}
