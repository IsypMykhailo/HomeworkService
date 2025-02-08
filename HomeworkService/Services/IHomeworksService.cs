using System.Linq.Expressions;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Models;

namespace HomeworkService.Services;

public interface IHomeworksService
{
    Task<Homeworks> CreateAsync(HomeworksDto dto, string teacherId, long groupId, long? subGroupId, CancellationToken ct = default);
    Task<Homeworks?> GetByIdAsync(long id, long groupId, long? subGroupId,
        Expression<Func<Homeworks, object>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, HomeworksDto dto, string teacherId, long groupId, long? subGroupId, CancellationToken ct = default);
    Task<IEnumerable<Homeworks>> GetAllAsync(long groupId, long? subGroupId, CancellationToken ct = default);

    Task<bool> IsUserTeacherAsync(string userId, CancellationToken ct = default);
}