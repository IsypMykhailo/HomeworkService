using System.Linq.Expressions;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Models;
using HomeworkService.Repositories;
using Mapster;

namespace HomeworkService.Services;

public class HomeworkItemsService : IHomeworkItemsService
{
    private readonly ICrudRepository<HomeworkItems> _homeworkItemsRepository;

    public HomeworkItemsService(ICrudRepository<HomeworkItems> homeworkItemsRepository)
    {
        _homeworkItemsRepository = homeworkItemsRepository;
    }

    public async Task<IEnumerable<HomeworkItems>> GetAllAsync(string teacherId, long groupId, long? subGroupId, CancellationToken ct = default)
    {
        if (subGroupId != null)
        {
            return await _homeworkItemsRepository.GetAsync(
                e => e.Homework.GroupId == groupId && e.Homework.SubGroupId == subGroupId && e.Homework.CreatorId == teacherId, ct: ct);
        }
        return await _homeworkItemsRepository.GetAsync(e => e.Homework.GroupId == groupId && e.Homework.CreatorId == teacherId, ct: ct);
    }
    
    public async Task<HomeworkItems?> GetByIdAsync(long homeworkId,
        string studentId, long groupId, long? subGroupId,
        Expression<Func<HomeworkItems, object>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        ;
        if (subGroupId != null)
        {
            return _homeworkItemsRepository.GetAsync(e =>
                    e.StudentId == studentId && e.Homework.Id == homeworkId && e.Homework.GroupId == groupId && e.Homework.SubGroupId == subGroupId,
                includeProperties: new Expression<Func<HomeworkItems, object>>[]
                {
                    e => e.Homework
                },
                ct).Result.First();
        }
        return _homeworkItemsRepository.GetAsync(e =>
                e.StudentId == studentId && e.Homework.Id == homeworkId && e.Homework.GroupId == groupId,
            includeProperties: new Expression<Func<HomeworkItems, object>>[]
            {
                e => e.Homework
            },
            ct).Result.First();
    }
    
    public async Task<HomeworkItems> CreateAsync(HomeworkItemsDto dto, Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = dto.Adapt<HomeworkItemsDto, HomeworkItems>();
        homeworkItem.Homework = homework;
        homeworkItem.StudentId = studentId;
        homeworkItem.HomeworkUploaded = utcDateTime;
        homeworkItem.HomeworkUpdated = utcDateTime;
        homeworkItem.Grade = 0;

        await _homeworkItemsRepository.CreateAsync(homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return homeworkItem;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;
        
        await _homeworkItemsRepository.DeleteAsync(homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> TeacherUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;

        homeworkItem.Grade = dto.Grade;
        homeworkItem.HomeworkUpdated = utcDateTime;

        await _homeworkItemsRepository.UpdateAsync(homeworkItem.Id, homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> StudentUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;

        var updatedHomeworkItem = dto.Adapt<HomeworkItemsDto, HomeworkItems>();
        updatedHomeworkItem.Grade = homeworkItem.Grade;
        updatedHomeworkItem.Id = homeworkItem.Id;
        updatedHomeworkItem.Homework = homework;
        updatedHomeworkItem.StudentId = studentId;
        updatedHomeworkItem.HomeworkUpdated = utcDateTime;

        await _homeworkItemsRepository.UpdateAsync(updatedHomeworkItem.Id, updatedHomeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> IsUserTeacherOrStudentAsync(string userId, CancellationToken ct = default)
    {
        if (await _homeworkItemsRepository.GetAsync(e => e.Homework.CreatorId == userId || e.StudentId == userId, ct: ct) != null)
        {
            return true;
        }

        return false;
    }
    
    public async Task<bool> IsUserStudentAsync(string userId, CancellationToken ct = default)
    {
        if (await _homeworkItemsRepository.GetAsync(e => e.StudentId == userId, ct: ct) != null)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> HomeworkItemExists(string teacherId, long homeworkId, long groupId, long? subGroupId, string userId, CancellationToken ct = default)
    {
        if (subGroupId != null)
        {
            if (_homeworkItemsRepository.GetAsync(e => e.Homework.Id == homeworkId && e.Homework.CreatorId == teacherId).Result.Any(e => e.StudentId == userId &&
                    e.Homework.Id == homeworkId))
                return true;
            return false;
        }
        else
        {
            if (_homeworkItemsRepository.GetAsync(e => e.Homework.Id == homeworkId && e.Homework.CreatorId == teacherId).Result.Any(e => e.StudentId == userId 
                                                                              && e.Homework.Id == homeworkId))
                return true;
            return false;
        }
    }
}