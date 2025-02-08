using HomeworkService.Domain.Models;
using HomeworkService.Domain.Response;

namespace HomeworkService.Domain.Mappers;

public class HomeworkItemsMapper
{
    public static HomeworkItemsResponse ToResponse(HomeworkItems obj)
    {
        return new()
        {
            Id = obj.Id,
            CompletedHomework = obj.CompletedHomework,
            HomeworkUploaded = obj.HomeworkUploaded,
            HomeworkUpdated = obj.HomeworkUpdated,
            Grade = obj.Grade,
            Comment = obj.Comment
        };
    }

    public static IEnumerable<HomeworkItemsResponse> ToResponses(IEnumerable<HomeworkItems> objEnumerable)
    {
        var items = objEnumerable as HomeworkItems[] ?? objEnumerable.ToArray();
        var responses = new HomeworkItemsResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}