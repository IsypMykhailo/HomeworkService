using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HomeworkService.Domain.Models;

public class HomeworkItems
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long HomeworkId { get; set; } = default!;

    [Required] public virtual Homeworks Homework { get; set; } = default!;

    public string StudentId { get; set; } = default!;

    public string CompletedHomework { get; set; } = default!;
    
    [Required] public DateTime HomeworkUploaded { get; set; }
    
    public DateTime? HomeworkUpdated { get; set; }
    
    public int? Grade { get; set; }
    
    public string? Comment { get; set; }
}