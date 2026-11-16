using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TutorPlatform.Api.Data;
using TutorPlatform.Core;
using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Auth;

public static class IdentitySeeder
{
    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<AppDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();

        
        foreach (var role in new[] { Roles.Student, Roles.Teacher })
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                var success = await roleManager.CreateAsync(new IdentityRole(role));
                if (!success.Succeeded)
                {
                    throw new InvalidOperationException($"{role} role creation failed. Errors: {success.Errors}");
                }
            }
        }
        
        var teacherEmail = configuration["Seed:TeacherEmail"];
        if (string.IsNullOrWhiteSpace(teacherEmail))
        {
            return;
        }
        
        var user =  await userManager.FindByEmailAsync(teacherEmail);
        if (user == null)
        {
            return;
        }
        
        if (!await userManager.IsInRoleAsync(user, Roles.Teacher))
        {
            var success =await userManager.AddToRoleAsync(user, Roles.Teacher);
            if (!success.Succeeded)
            {
                throw new InvalidOperationException($"Adding user to teacher role failed. Errors: {success.Errors}");
            }
        }
        
        if (!await db.Teachers.AnyAsync(t => t.UserId == user.Id))
        {
            db.Teachers.Add(new Teacher(user.Id, user.UserName));
            await db.SaveChangesAsync();
        }
    }
}