namespace HomeworkService.Domain.Response;

public class HomeworkItemsResponse
{
    public long Id { get; set; }
    
    public string CompletedHomework { get; set; } = default!;

    public DateTime HomeworkUploaded { get; set; } = default!;

    public DateTime? HomeworkUpdated { get; set; } = default!;

    public int? Grade { get; set; } = default!;

    public string? Comment { get; set; } = default!;
}