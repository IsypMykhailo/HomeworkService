using System.Linq.Expressions;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Models;
using HomeworkService.Repositories;
using Mapster;

namespace HomeworkService.Services;

public class HomeworksService : IHomeworksService
{
    private readonly ICrudRepository<Homeworks> _homeworksRepository;

    public HomeworksService(ICrudRepository<Homeworks> homeworksRepository)
    {
        _homeworksRepository = homeworksRepository;
    }
    
    public async Task<IEnumerable<Homeworks>> GetAllAsync(long groupId, long? subGroupId, CancellationToken ct = default)
    {
        if (subGroupId != null)
        {
            return await _homeworksRepository.GetAsync(e => e.SubGroupId == subGroupId && e.GroupId == groupId, ct: ct);
        }
        return await _homeworksRepository.GetAsync(e => e.GroupId == groupId, ct: ct);
    }

    public async Task<Homeworks> CreateAsync(HomeworksDto dto, string teacherId, long groupId, long? subGroupId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homework = dto.Adapt<HomeworksDto, Homeworks>();
        homework.CreatorId = teacherId;
        homework.GroupId = groupId;
        homework.SubGroupId = subGroupId ?? null;
        homework.UploadedDate = utcDateTime;
        homework.DueDate = dto.DueDate.ToUniversalTime();

        await _homeworksRepository.CreateAsync(homework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return homework;
    }
    
    public async Task<Homeworks?> GetByIdAsync(long id, long groupId, long? subGroupId,
        Expression<Func<Homeworks, object>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        if (subGroupId != null)
        {
            return await _homeworksRepository.GetByIdAsync(id, 
                e => e.GroupId == groupId && e.SubGroupId == subGroupId,
                includeProperties,
                ct);
        }
        return await _homeworksRepository.GetByIdAsync(id, 
            e => e.GroupId == groupId,
            includeProperties,
            ct);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var homework = await _homeworksRepository.GetByIdAsync(id, ct: ct);

        if (homework is null)
            return false;
        
        await _homeworksRepository.DeleteAsync(homework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, HomeworksDto dto, string teacherId, long groupId, long? subGroupId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homework = await _homeworksRepository.GetByIdAsync(id, ct: ct);

        if (homework is null)
            return false;
        
        var updatedHomework = dto.Adapt<HomeworksDto, Homeworks>();
        updatedHomework.CreatorId = teacherId;
        updatedHomework.GroupId = groupId;
        updatedHomework.SubGroupId = subGroupId;
        updatedHomework.UploadedDate = utcDateTime;
        updatedHomework.DueDate = dto.DueDate.ToUniversalTime();
        updatedHomework.Id = homework.Id;

        await _homeworksRepository.UpdateAsync(updatedHomework.Id, updatedHomework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> IsUserTeacherAsync(string userId, CancellationToken ct = default)
    {
        if (await _homeworksRepository.GetAsync(e => e.CreatorId == userId, ct: ct) != null)
        {
            return true;
        }

        return false;
    }
}