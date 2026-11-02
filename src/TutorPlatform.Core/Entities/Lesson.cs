using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Entities;

public class Lesson
{
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid StudentId { get; private set; }
    public LessonStatus Status { get; private set; }
    public DateTime Start { get; private set; }
    public TimeSpan Duration { get; private set; }
    public DateTime End => Start + Duration;

    public bool IsAwaitingMark(DateTime now) => Status == LessonStatus.Planned && now > End + TimeSpan.FromMinutes(30);

    public Lesson(Guid teacherId, Guid studentId, DateTime date, TimeSpan duration)
    {
        Id = Guid.CreateVersion7();
        if (teacherId == Guid.Empty)
        {
            throw new DomainException("Teacher id cannot be empty.");
        }
        TeacherId = teacherId;
        if (studentId == Guid.Empty)
        {
            throw new DomainException("Student id cannot be empty.");
        }
        StudentId = studentId;
        Start = date;
        EnsureCorrectDuration(duration);
        Duration = duration;
        Status = LessonStatus.Planned;
    }

    private Lesson() { }

    public void Complete()
    {
        EnsurePlanned();
        Status = LessonStatus.Completed;
    }

    public void Cancel()
    {
        EnsurePlanned();
        Status = LessonStatus.Canceled;
    }

    public void Reschedule(DateTime newStart, TimeSpan newDuration)
    {
        EnsurePlanned();
        EnsureCorrectDuration(newDuration);
        Start = newStart;
        Duration = newDuration;
    }

    private void EnsurePlanned()
    {
        if (Status != LessonStatus.Planned)
        {
            throw new DomainException("Only planned lesson may be changed.");
        }
    }
    
    private void EnsureCorrectDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new DomainException("Duration must be greater than zero.");
        }
    }
}