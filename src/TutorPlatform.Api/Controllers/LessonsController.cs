using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Contracts;
using TutorPlatform.Api.Data;
using TutorPlatform.Core;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LessonsController (AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LessonResponse>>> Get(
        [FromQuery] Guid teacherId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from >= to)
        {
            return BadRequest("Invalid time period");
        }

        if (teacherId == Guid.Empty)
        {
            return BadRequest("Teacher Id is required");
        }
        
        var lessons = await db.Lessons
            .AsNoTracking()
            .Where(l => l.TeacherId == teacherId && l.Start >= from && l.Start < to)
            .OrderBy(l => l.Start)
            .Select (l => LessonResponse.From(l))
            .ToListAsync();
    
        return Ok(lessons);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LessonResponse>> GetById(Guid id)
    {
        var lesson = await db.Lessons
            .AsNoTracking()
            .Where (l => l.Id == id)
            .Select (l => LessonResponse.From(l))
            .FirstOrDefaultAsync();

        return lesson is null ? NotFound() : Ok(lesson);

    }
    
    [HttpPost]
    public async Task<ActionResult<LessonResponse>> Create(CreateLessonRequest request)
    {
        var studentExists = await db.Students.AnyAsync(s => s.Id == request.StudentId);
        if (!studentExists)
        {
            return NotFound("Student not found");
        }
        
        var teacherExists = await db.Teachers.AnyAsync(t => t.Id == request.TeacherId);
        if (!teacherExists)
        {
            return NotFound("Teacher not found");
        }
        
        var belongs = await db.Students
            .AnyAsync(s => s.Id == request.StudentId && s.Teacher!.Id == request.TeacherId);
        
        if (!belongs)
        {
            return BadRequest("This student's teacher differs from provided");
        }

        var lesson = new Lesson(request.TeacherId, request.StudentId, request.Start, TimeSpan.FromMinutes(request.DurationMinutes));

        var existing = await LoadLessonsAround(lesson.TeacherId, lesson.Start);
        
        LessonScheduler.EnsureNoConflicts(lesson, existing);
        
        db.Lessons.Add(lesson);
        await db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, LessonResponse.From(lesson));
    }
    
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<LessonResponse>> Complete(Guid id)
    {
        var lesson = await db.Lessons.FindAsync(id);
        if (lesson is null)
        {
            return NotFound();
        }

        lesson.Complete();
        await db.SaveChangesAsync();

        return Ok(LessonResponse.From(lesson));
    }
    
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<LessonResponse>> Cancel(Guid id)
    {
        var lesson = await db.Lessons.FindAsync(id);
        if (lesson is null)
        {
            return NotFound();
        }

        lesson.Cancel();
        await db.SaveChangesAsync();

        return Ok(LessonResponse.From(lesson));
    }

    [HttpPost("{id:guid}/reschedule")]
    public async Task<ActionResult<LessonResponse>> Reschedule(Guid id, RescheduleLessonRequest request)
    {
        var lesson = await db.Lessons.FindAsync(id);
        if (lesson is null)
        {
            return NotFound();
        }
        
        lesson.Reschedule(request.Start, TimeSpan.FromMinutes(request.DurationMinutes));
        
        var existing = await LoadLessonsAround(lesson.TeacherId, lesson.Start);
        
        LessonScheduler.EnsureNoConflicts(lesson, existing);
        
        await db.SaveChangesAsync();
        return Ok(LessonResponse.From(lesson));
    }

    private async Task<List<Lesson>> LoadLessonsAround(Guid teacherId, DateTime start)
    {
        var from = start.AddDays(-1);
        var to = start.AddDays(1);

        return await db.Lessons
            .AsNoTracking()
            .Where(l => l.TeacherId == teacherId && l.Start >= from && l.Start < to)
            .ToListAsync();
    }
}