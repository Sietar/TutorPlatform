namespace TutorPlatform.Core.Entities;

public class Student
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Name { get; private set; }
    
    public Teacher? Teacher { get; private set; }
    
    private Student()
    {
        UserId = null!;
        Name = null!;
    }
    
    public Student(string userId, string name)
    {
        Id = Guid.CreateVersion7();
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("User id cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name cannot be null or empty.");
        }
        UserId = userId;
        Name = name;
    }
    
    internal void SetTeacher(Teacher teacher)
    {
        Teacher = teacher ?? throw new DomainException("Teacher cannot be null.");
    }

    internal void RemoveTeacher()
    {
        Teacher = null;
    }
}