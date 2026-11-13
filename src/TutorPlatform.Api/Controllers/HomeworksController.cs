using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Contracts;
using TutorPlatform.Api.Data;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class HomeworksController (AppDbContext db) : ControllerBase
{
    private readonly DateTime _now = DateTime.UtcNow;
    
    [HttpGet]
    public async Task<ActionResult<List<HomeworkResponse>>> Get(
        [FromQuery] Guid? studentId, [FromQuery] Guid? teacherId)
    {
        if (studentId.Equals(Guid.Empty) &&  teacherId.Equals(Guid.Empty))
        {
            return BadRequest("Student id is required");
        }
        
        var homeworks = await db.Homeworks
            .AsNoTracking()
            .Where(h => h.StudentId == studentId || h.TeacherId == teacherId)
            .OrderBy(h => h.DueDate)
            .Select(l => HomeworkResponse.From(l, _now))
            .ToListAsync();
        
        return Ok(homeworks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HomeworkResponse>> GetById(Guid id)
    {
        var homework = await db.Homeworks
            .AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => HomeworkResponse.From(h, _now))
            .FirstOrDefaultAsync();
        
        return homework is null ? NotFound() : Ok(homework);
    }
    
    [HttpPost]
    public async Task<ActionResult<HomeworkResponse>> Create(CreateHomeworkRequest request)
    {
        var teacherExists = await db.Teachers.AnyAsync(t => t.Id == request.TeacherId);
        if (!teacherExists)
        {
            return NotFound("Teacher not found");
        }
        
        var studentExists = await db.Students.AnyAsync(s => s.Id == request.StudentId);
        if (!studentExists)
        {
            return NotFound("Student not found");
        }
        
        var belongs = await db.Students.AnyAsync(s => s.Id == request.StudentId && s.Teacher!.Id == request.TeacherId);
        if (!belongs)
        {
            return BadRequest("This student's teacher differs from provided");
        }

        var homework = new Homework(request.TeacherId, request.StudentId, request.Title, request.Description,
            DateTime.UtcNow, request.DueDate, request.MaterialsUrl);
        
        db.Homeworks.Add(homework);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById),  new { id = homework.Id }, HomeworkResponse.From(homework, _now));
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<HomeworkResponse>> Submit(Guid id, SubmitRequest request)
    {
        var homework = await db.Homeworks.FindAsync(id);
        if (homework is null)
        {
            return NotFound("Homework with such id not found");
        }
        
        homework.Submit(request.Answer);
        await db.SaveChangesAsync();
        
        return Ok(HomeworkResponse.From(homework, _now));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<HomeworkResponse>> Complete(Guid id)
    {
        var homework = await db.Homeworks.FindAsync(id);
        if (homework is null)
        {
            return NotFound("Homework with such id not found");
        }
        
        homework.Complete();
        await db.SaveChangesAsync();
        
        return Ok(HomeworkResponse.From(homework, _now));
    }
    
    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<HomeworkResponse>> Return(Guid id, ReturnRequest request)
    {
        var homework = await db.Homeworks.FindAsync(id);
        if (homework is null)
        {
            return NotFound("Homework with such id not found");
        }
        
        homework.Return(request.comment);
        await db.SaveChangesAsync();
        
        return Ok(HomeworkResponse.From(homework, _now));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<HomeworkResponse>> Cancel(Guid id)
    {
        var homework = await db.Homeworks.FindAsync(id);
        if (homework is null)
        {
            return NotFound("Homework with such id not found");
        }
        
        homework.Cancel();
        await db.SaveChangesAsync();
        
        return Ok(HomeworkResponse.From(homework, _now));
    }
}