using System.Linq.Expressions;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Models;

namespace HomeworkService.Services;

public interface IHomeworkItemsService
{
    Task<HomeworkItems> CreateAsync(HomeworkItemsDto dto, Homeworks homeworks, string studentId, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> TeacherUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homeworks, string studentId, CancellationToken ct = default);

    Task<bool> StudentUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homeworks, string studentId, CancellationToken ct = default);

    Task<IEnumerable<HomeworkItems>> GetAllAsync(string teacherId, long groupId, long? subGroupId,
        CancellationToken ct = default);

    Task<HomeworkItems?> GetByIdAsync(long homeworkId,
        string studentId, long groupId, long? subGroupId,
        Expression<Func<HomeworkItems, object>>[]? includeProperties = null,
        CancellationToken ct = default);

    Task<bool> IsUserTeacherOrStudentAsync(string userId, CancellationToken ct = default);
    Task<bool> IsUserStudentAsync(string userId, CancellationToken ct = default);

    Task<bool> HomeworkItemExists(string teacherId, long homeworkId, long groupId, long? subGroupId, string userId,
        CancellationToken ct = default);
}