using TutorPlatform.Core.Entities;
using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core;

public static class LessonScheduler
{
    public static void EnsureNoConflicts(Lesson lesson, IEnumerable<Lesson> lessonsPlanned)
    {
        foreach (var existing in lessonsPlanned)
        {
            if (existing.Status == LessonStatus.Canceled)
            {
                continue;
            }
            if (lesson.Start < existing.End && existing.Start < lesson.End)
            {
                throw new DomainException("Lessons can't overlap");
            }
        }
    }
}