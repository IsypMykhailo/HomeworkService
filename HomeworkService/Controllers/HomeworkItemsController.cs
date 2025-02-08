using System.Linq.Expressions;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Mappers;
using HomeworkService.Domain.Models;
using HomeworkService.Domain.Response;
using HomeworkService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeworkService.Controllers;

[ApiController]
[Route("api/v1/groups/{groupId:long}")]
public class HomeworkItemsController : ControllerBase
{
    private readonly IHomeworksService _homeworksService;
    private readonly IHomeworkItemsService _homeworkItemsService;

    public HomeworkItemsController(IHomeworksService homeworksService, IHomeworkItemsService homeworkItemsService)
    {
        _homeworksService = homeworksService;
        _homeworkItemsService = homeworkItemsService;
    }
    
    [HttpGet("{subGroupId:long}/homeworks/{homeworkId:long}/all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(long groupId, long subGroupId, long homeworkId, CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId,
            groupId, subGroupId,
            new Expression<Func<Homeworks, object>>[]
            {
                e => e.Items
            }, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();

        return Ok(HomeworkItemsMapper.ToResponses(homework.Items));
    }
    
    [HttpGet("homeworks/{homeworkId:long}/all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(long groupId, long homeworkId, CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId,
            groupId, null,
            new Expression<Func<Homeworks, object>>[]
            {
                e => e.Items
            }, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");
        
        if (homework.SubGroupId != null)
            return Forbid();
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();

        return Ok(HomeworkItemsMapper.ToResponses(homework.Items));
    }
    
    [HttpGet("{subGroupId:long}/homeworks/{homeworkId:long}/students/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(string studentId, long groupId, long subGroupId, long homeworkId, CancellationToken ct)
    {
        var homework = await _homeworkItemsService.GetByIdAsync(
            homeworkId, studentId, groupId, subGroupId, ct: ct);

        if (homework is null)
            return NotFound($"HomeworkItem by id {homework.Id} is not found");
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        return Ok(HomeworkItemsMapper.ToResponse(homework));
    }
    
    [HttpGet("homeworks/{homeworkId:long}/students/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOne(string studentId, long groupId, long homeworkId, CancellationToken ct)
    {
        var homework = await _homeworkItemsService.GetByIdAsync(
            homeworkId, studentId, groupId, null, ct: ct);

        if (homework is null)
            return NotFound($"HomeworkItem by id {homework.Id} is not found");

        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        return Ok(HomeworkItemsMapper.ToResponse(homework));
    }

    [HttpPost("{subGroupId:long}/homeworks/{homeworkId:long}/create")]
    [ProducesResponseType(typeof(HomeworkItemsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(long groupId, long subGroupId, long homeworkId, HomeworkItemsDto dto, 
        CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, subGroupId, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");

        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserStudentAsync(userId, ct))
            return Forbid();

        if (await _homeworkItemsService.HomeworkItemExists(homework.CreatorId, homeworkId, groupId, subGroupId, userId, ct: ct))
            return Forbid();
        
        var homeworkItem = await _homeworkItemsService.CreateAsync(dto, homework, userId, ct);
        return Created(Request.Path.Value!, HomeworkItemsMapper.ToResponse(homeworkItem));
    }
    
    [HttpPost("homeworks/{homeworkId:long}/create")]
    [ProducesResponseType(typeof(HomeworkItemsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(long groupId, long homeworkId, HomeworkItemsDto dto, 
        CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, null, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");
        
        if (homework.SubGroupId != null)
            return Forbid();
        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserStudentAsync(userId, ct))
            return Forbid();

        if (await _homeworkItemsService.HomeworkItemExists(homework.CreatorId, homeworkId, groupId, null, userId, ct: ct))
            return Forbid();

        var homeworkItem = await _homeworkItemsService.CreateAsync(dto, homework, userId, ct);
        return Created(Request.Path.Value!, HomeworkItemsMapper.ToResponse(homeworkItem));
    }

    [HttpPut("{subGroupId:long}/homeworks/{homeworkId:long}/update/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long groupId, long subGroupId, long homeworkId,
        long homeworkItemId, HomeworkItemsDto dto, CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, subGroupId, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        if (await _homeworksService.IsUserTeacherAsync(userId, ct))
        {
            var updated = await _homeworkItemsService.TeacherUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Ok() : NotFound($"HomeworkItem by id {homeworkItemId} is not found");
        }
        else
        {
            var updated = await _homeworkItemsService.StudentUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Ok() : NotFound($"HomeworkItem by id {homeworkItemId} is not found");

        }
    }
    
    [HttpPut("homeworks/{homeworkId:long}/update/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long groupId, long homeworkId,
        long homeworkItemId, HomeworkItemsDto dto, CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, null, ct: ct);
        
        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");
        
        if (homework.SubGroupId != null)
            return Forbid();
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        if (await _homeworksService.IsUserTeacherAsync(userId, ct))
        {
            var updated = await _homeworkItemsService.TeacherUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Ok() : NotFound($"HomeworkItem by id {homeworkItemId} is not found");
        }
        else
        {
            var updated = await _homeworkItemsService.StudentUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Ok() : NotFound($"HomeworkItem by id {homeworkItemId} is not found");

        }
    }

    [HttpDelete("{subGroupId:long}/homeworks/{homeworkId:long}/delete/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long groupId, long subGroupId, long homeworkId, long homeworkItemId,
        CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, subGroupId, ct: ct);

        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");

        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        var deleted = await _homeworkItemsService.DeleteAsync(homeworkItemId, ct);
        return deleted ? Ok() : NotFound($"Homework Item by id {homeworkItemId} is not found");
    }
    
    [HttpDelete("homeworks/{homeworkId:long}/delete/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long groupId, long homeworkId, long homeworkItemId,
        CancellationToken ct)
    {
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, null, ct: ct);

        if (homework is null)
            return NotFound($"Homework by id {homeworkId} is not found");

        if (homework.SubGroupId != null)
            return Forbid();
        
        var userId = "0000-0000-0000-0000";

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Forbid();

        var deleted = await _homeworkItemsService.DeleteAsync(homeworkItemId, ct);
        return deleted ? Ok() : NotFound($"Homework Item by id {homeworkItemId} is not found");
    }
}