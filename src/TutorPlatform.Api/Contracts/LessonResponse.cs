using TutorPlatform.Core.Entities;
using TutorPlatform.Core.Enums;

namespace TutorPlatform.Api.Contracts;

public record LessonResponse(
    Guid Id,
    LessonStatus Status,
    Guid TeacherId,
    Guid StudentId,
    DateTime Start,
    int DurationMinutes)
{

    public static LessonResponse From(Lesson lesson)
    {
        return new LessonResponse(lesson.Id, lesson.Status, lesson.TeacherId, lesson.StudentId, lesson.Start,
            (int)lesson.Duration.TotalMinutes);
    }

}