using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Auth;
using TutorPlatform.Api.Contracts;
using TutorPlatform.Api.Data;

namespace TutorPlatform.Api.Controllers;

[ApiController]
[Route("api/me")]
public class ProfilesController(
    AppDbContext db,
    CurrentUser currentUser,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userId = currentUser.UserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        var teacherProfile = await db.Teachers
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => new ProfileResponse(userId, t.Id, t.Name, user.Email, Roles.Teacher))
            .FirstOrDefaultAsync();

        if (teacherProfile is not null)
        {
            return Ok(teacherProfile);
        }

        var studentProfile = await db.Students
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new ProfileResponse(userId, s.Id, s.Name, user.Email, Roles.Student))
            .FirstOrDefaultAsync();

        if (studentProfile is not null)
        {
            return Ok(studentProfile);
        }

        return Ok(new ProfileResponse(userId, null, user.Email, null, null));
    }
}