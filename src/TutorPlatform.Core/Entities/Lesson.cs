using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Entities;

public class Lesson
{
    public Guid TeacherId { get; private set; }
    public Guid StudentId { get; private set; }
    public LessonStatus Status { get; private set; }
    public DateTime Start { get; private set; }
    public TimeSpan Duration { get; private set; }
    public DateTime End => Start + Duration;

    public Lesson(Guid teacherId, Guid studentId, DateTime date, TimeSpan duration)
    {
        TeacherId = teacherId;
        StudentId = studentId;
        Start = date;
        Duration = duration;
        Status = LessonStatus.Planned;
    }

    public void Cancel()
    {
        Status = LessonStatus.Canceled;
    }
}