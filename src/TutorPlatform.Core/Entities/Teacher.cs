namespace TutorPlatform.Core.Entities;

public class Teacher
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Name { get; private set; }
    
    private readonly List<Student> _students = [];
    public IReadOnlyCollection<Student> Students => _students;
    
    private Teacher()
    {
        UserId = null!;
        Name = null!;
    }
    
    public Teacher(string userId, string name)
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

    public void AddStudent(Student student)
    {
        if (student is null)
        {
            throw new DomainException("Student cannot be null.");
        }
        if (_students.Any(s => s.Id == student.Id))
        {
            throw new DomainException("This student already exists.");
        }

        if (student.Teacher is null)
        {
            student.SetTeacher(this);
            _students.Add(student);
        }
        else
        {
            throw new DomainException("This student already has a teacher.");
        }
    }

    public void RemoveStudent(Guid studentId)
    {
        Student? student = _students.FirstOrDefault(s => s.Id == studentId);
        if (student is null) 
        { 
            throw new DomainException("This student doesn't exist."); 
        }
        if (student.Teacher is null || student.Teacher.Id != Id)
        {
            throw new DomainException("This student has different or no teacher.");
        }
        
        _students.Remove(student);
        student.RemoveTeacher();
        
    }
    
}