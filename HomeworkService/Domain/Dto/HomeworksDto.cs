namespace HomeworkService.Domain.Dto;

public class HomeworksDto
{
    public string Title { get; set; } = default!;
    
    public string? HomeworkPath { get; set; } = default!;

    public string? Description { get; set; } = default!;

    public DateTime UploadedDate { get; set; } = default!;

    public DateTime DueDate { get; set; } = default!;

    public int MaxGrade { get; set; } = default!;
}