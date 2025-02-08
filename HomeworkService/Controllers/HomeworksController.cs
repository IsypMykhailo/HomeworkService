using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using HomeworkService.Domain.Dto;
using HomeworkService.Domain.Mappers;
using HomeworkService.Domain.Response;
using HomeworkService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeworkService.Controllers;

[ApiController]
[Route("api/v1/groups/{groupId:long}")]
public class HomeworksController : ControllerBase
{
    private readonly IHomeworksService _homeworksService;

    public HomeworksController(IHomeworksService homeworksService)
    {
        _homeworksService = homeworksService;
    }

    [HttpGet("{subGroupId:long}/homeworks/all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworksResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long groupId, long subGroupId, CancellationToken ct)
    {
        var homeworks = await _homeworksService.GetAllAsync(groupId, subGroupId, ct);
        return Ok(HomeworksMapper.ToResponses(homeworks));
    }
    
    [HttpGet("homeworks/all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworksResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long groupId, CancellationToken ct)
    {
        var homeworks = await _homeworksService.GetAllAsync(groupId, null, ct);
        return Ok(HomeworksMapper.ToResponses(homeworks));
    }

    [HttpPost("{subGroupId:long}/homeworks/create")]
    [ProducesResponseType(typeof(HomeworksResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(long groupId, long subGroupId, HomeworksDto homeworksDto, CancellationToken ct)
    {
        var userId = "0000-0000-0000-0000";

        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();
        var homework = await _homeworksService.CreateAsync(homeworksDto, userId, groupId, subGroupId, ct);
        return Created(Request.Path.Value!, HomeworksMapper.ToResponse(homework));
    }
    
    [HttpPost("homeworks/create")]
    [ProducesResponseType(typeof(HomeworksResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(long groupId, HomeworksDto homeworksDto, CancellationToken ct)
    {
        var userId = "0000-0000-0000-0000";

        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();
        var homework = await _homeworksService.CreateAsync(homeworksDto, userId, groupId, null, ct);
        return Created(Request.Path.Value!, HomeworksMapper.ToResponse(homework));
    }

    [HttpPut("homeworks/update/{homeworkId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long homeworkId, long groupId, HomeworksDto homeworksDto, CancellationToken ct)
    {
        var userId = "0000-0000-0000-0000";
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();
        
        var updated = await _homeworksService.UpdateAsync(homeworkId, homeworksDto, userId, groupId, null, ct);
        return updated ? Ok() : NotFound($"Homework by id {homeworkId} is not found");
    }
    
    [HttpPut("{subGroupId:long}/homeworks/update/{homeworkId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long homeworkId, long groupId, long subGroupId, HomeworksDto homeworksDto, CancellationToken ct)
    {
        var userId = "0000-0000-0000-0000";
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();
        
        var updated = await _homeworksService.UpdateAsync(homeworkId, homeworksDto, userId, groupId, subGroupId, ct);
        return updated ? Ok() : NotFound($"Homework by id {homeworkId} is not found");
    }
    
    [HttpDelete("homeworks/delete/{homeworkId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long homeworkId, CancellationToken ct)
    {
        var userId = "0000-0000-0000-0000";
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Forbid();
        
        var deleted = await _homeworksService.DeleteAsync(homeworkId, ct);
        return deleted ? Ok() : NotFound($"Homework by id {homeworkId} is not found");
    }
}