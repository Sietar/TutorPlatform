using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Entities;

public class Homework
{
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid StudentId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime DueDate { get; private set; }
    public HomeworkStatus Status { get; private set; }
    public string? Answer { get; private set; }
    public string? Comment { get; private set; }
    public string? MaterialsUrl { get; private set; }
    public bool IsOverdue (DateTime now) => (Status == HomeworkStatus.Assigned || Status == HomeworkStatus.Returned) && now > DueDate;

    public Homework(Guid teacherId, Guid studentId, string title, string? description, DateTime createdAt, DateTime dueDate, string materialsUrl = null)
    {
        Id = Guid.CreateVersion7();
        if (teacherId == Guid.Empty)
        {
            throw new DomainException("Teacher id cannot be empty");
        }
        TeacherId = teacherId;
        if (studentId == Guid.Empty)
        {
            throw new DomainException("Student id cannot be empty");
        }
        StudentId = studentId;
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Title can't be empty");
        }
        Title = title;
        Description = description;
        if (createdAt.Kind != DateTimeKind.Utc || dueDate.Kind != DateTimeKind.Utc)
        {
            throw new DomainException("Time must be UTC.");
        }

        if (createdAt > dueDate)
        {
            throw new DomainException("Created time can't be after due to time.");
        }
        CreatedAt = createdAt;
        DueDate = dueDate;
        MaterialsUrl = materialsUrl;
        Status = HomeworkStatus.Assigned;
    }

    private Homework()
    {
        Title = null!;
    }

    public void Submit(string answer)
    {
        if (Status is not (HomeworkStatus.Assigned or HomeworkStatus.Returned))
        {
            throw new DomainException("Only assigned or returned homework can be submitted.");
        }
        
        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new DomainException("Answer can't be empty");
        }
        Answer = answer;
        Status = HomeworkStatus.Submitted;
    }

    public void Return(string comment)
    {
        if (Status != HomeworkStatus.Submitted)
        {
            throw new DomainException("Only submitted homework can be returned.");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new DomainException("Comment can't be empty");
        }
        Comment = comment;
        Status = HomeworkStatus.Returned;
    }

    public void Complete()
    {
        if (Status != HomeworkStatus.Submitted)
        {
            throw new DomainException("Only submitted homework can be marked as completed.");
        }
        Status = HomeworkStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == HomeworkStatus.Completed)
        {
            throw new DomainException("Completed homework cannot be cancelled.");
        }

        if (Status == HomeworkStatus.Canceled)
        {
            throw new DomainException("The homework is already canceled.");
        } 
        Status = HomeworkStatus.Canceled;
    }
}