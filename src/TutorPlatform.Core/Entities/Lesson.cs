using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Entities;

public class Lesson
{
    public LessonStatus Status { get; private set; }
    public DateTime Date { get; private set; }
}