using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Contracts;
using TutorPlatform.Api.Data;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController (AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TeacherResponse>>> GetAll()
    {
        var teachers = await db.Teachers
            .AsNoTracking()
            .Select(t => new TeacherResponse(t.Id, t.Name))
            .ToListAsync();

        return Ok(teachers);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeacherResponse>> GetById(Guid id)
    {
        var teacher = await db.Teachers
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TeacherResponse(t.Id, t.Name))
            .FirstOrDefaultAsync();

        return teacher is null ? NotFound() : Ok(teacher);
    }

    [HttpGet("{teacherId:guid}/students")]
    public async Task<ActionResult<List<StudentResponse>>> GetStudents(Guid teacherId)
    {
        if (!await db.Teachers.AnyAsync(t => t.Id == teacherId))
        {
            return NotFound();
        }
        
        var students = await db.Students
            .AsNoTracking()
            .Where(s => s.Teacher!.Id == teacherId)
            .Select(s => new StudentResponse(s.Id, s.Name))
            .ToListAsync();
        
        
        
        return Ok(students);
    }

    [HttpPost]
    public async Task<ActionResult<TeacherResponse>> Create(CreateTeacherRequest request)
    {
        var teacher = new Teacher(request.UserId, request.Name);
        
        db.Add(teacher);
        await db.SaveChangesAsync();
        var response = new TeacherResponse(teacher.Id, teacher.Name);
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, response);
    }

    [HttpPost("{teacherId:guid}/students")]
    public async Task<ActionResult<StudentResponse>> AddStudent(Guid teacherId, AddStudentRequest request)
    {
        var teacher = await db.Teachers
            .Include(t => t.Students)
            .FirstOrDefaultAsync(t => t.Id == teacherId); 

        if (teacher is null)
        {
            return NotFound();
        }

        var student = new Student(request.UserId, request.Name);
        
        teacher.AddStudent(student);

        await db.SaveChangesAsync();
        return Ok(new StudentResponse(student.Id, student.Name));
    }
}