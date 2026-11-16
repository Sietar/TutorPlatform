using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Auth;
using TutorPlatform.Api.Contracts;
using TutorPlatform.Api.Data;
using System.Transactions;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController(
    AppDbContext db,
    CurrentUser currentUser,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
    public async Task<ActionResult<List<TeacherResponse>>> GetAll()
    {
        var teachers = await db.Teachers
            .AsNoTracking()
            .Select(t => new TeacherResponse(t.Id, t.Name))
            .ToListAsync();

        return Ok(teachers);
    }
    
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
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
    [Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
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

    [HttpPost("me/students")]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<StudentResponse>> AddStudent(AddStudentRequest request)
    {
        var teacherId = await currentUser.GetTeacherIdAsync();
        if (teacherId is null)
        {
            return Forbid();
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
        {
            return NotFound("User not found");
        }
        
        if (user.Id == currentUser.UserId)
        {
            return BadRequest("You cannot add yourself as student");
        }

        var studentExists = await db.Students.AnyAsync(s => s.UserId == user.Id);
        if (studentExists)
        {
            return Conflict("Student already exists");
        }
        

        var teacher = await db.Teachers
            .Where(t => t.Id == teacherId).Include(t => t.Students)
            .FirstOrDefaultAsync();
            
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        var student = new Student(user.Id, request.Name);
        teacher.AddStudent(student);
            
        await db.SaveChangesAsync();
            
        var success = await userManager.AddToRoleAsync(user, Roles.Student);
        if (!success.Succeeded)
        {
            return Problem();
        }
        
        await transaction.CommitAsync();

        return Ok(StudentResponse.From(student));

    }
}