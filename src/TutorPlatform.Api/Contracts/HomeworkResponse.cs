using TutorPlatform.Core.Entities;
using TutorPlatform.Core.Enums;

namespace TutorPlatform.Api.Contracts;

public record HomeworkResponse(
    Guid Id,
    Guid TeacherId,
    Guid StudentId,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime DueDate,
    HomeworkStatus Status,
    string Answer = "",
    string Comment = "",
    string MaterialsUrl = "")
{
    public static HomeworkResponse From(Homework homework)
    {
        return new HomeworkResponse(homework.Id, homework.TeacherId, homework.StudentId, homework.Title, homework.Description,
            homework.CreatedAt, homework.DueDate, homework.Status, homework.Answer, homework.Comment, homework.MaterialsUrl);
    }
}