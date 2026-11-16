using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Data;

namespace TutorPlatform.Api.Auth;

public class CurrentUser
{
    public string? UserId { get; }
    private readonly AppDbContext _db;

    public CurrentUser(IHttpContextAccessor httpContextAccessor, AppDbContext db)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;

        _db = db;
        
        if (claimsPrincipal is null) return;

        UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<Guid?> GetTeacherIdAsync()
    {
        return await _db.Teachers
            .Where(t => t.UserId == UserId)
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Guid?> GetStudentIdAsync()
    {
        return await _db.Students
            .Where(s => s.UserId == UserId)
            .Select(s => (Guid?)s.Id)
            .FirstOrDefaultAsync();
    }
}
