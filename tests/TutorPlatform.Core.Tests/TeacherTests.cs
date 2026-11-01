using TutorPlatform.Core.Entities;

namespace TutorPlatform.Core.Tests;

public class TeacherTests
{
    [Fact]
    public void Constructor_WhenValid_SetsProperties()
    {
        var teacher = new Teacher("user-1", "Anna");

        Assert.NotEqual(Guid.Empty, teacher.Id);
        Assert.Equal("Anna", teacher.Name);
        Assert.Equal("user-1", teacher.UserId);
        Assert.Empty(teacher.Students);
    }

    [Fact]
    public void AddStudent_WhenValid_AddsStudentAndSetsTeacher()
    {
        var teacher = new Teacher("user-1", "Anna");
        var student = new Student("user-2", "Alex");
        
        teacher.AddStudent(student);
        var added = Assert.Single(teacher.Students);
        Assert.Same(student, added);
        Assert.Same(teacher, student.Teacher);
    }

    [Fact]
    public void RemoveStudent_WhenValid_RemovesStudentAndTeacher()
    {
        var teacher = new Teacher("user-1", "Anna");
        var student = new Student("user-2", "Alex");
        
        teacher.AddStudent(student);
        teacher.RemoveStudent(student.Id);
        
        Assert.Null(student.Teacher);
        Assert.DoesNotContain(student, teacher.Students);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenIdInvalid_Throws(string? id)
    {
        Assert.Throws<DomainException>(() => new Teacher(id!, "Anna"));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WhenNameInvalid_Throws(string? name)
    {
        Assert.Throws<DomainException>(() => new Teacher("user-1", name!));
    }
    
    [Fact]
    public void AddStudent_WhenStudentAlreadyAdded_Throws()
    {
        var teacher = new Teacher("user-1", "Anna");
        var student = new Student("user-2", "Petr");
        
        teacher.AddStudent(student);
        
        Assert.Throws<DomainException>(() => teacher.AddStudent(student));
    }
    
    [Fact]
    public void AddStudent_WhenStudentHasAnotherTeacher_Throws()
    {
        var teacher = new Teacher("user-1", "Anna");
        var teacher2 = new Teacher("user-2", "Alex");
        var student = new Student("user-2", "Petr");
        
        teacher.AddStudent(student);
        Assert.Throws<DomainException> (() => teacher2.AddStudent(student));
    }

    
    [Fact]
    public void RemoveStudent_WhenHasNoStudent_Throws()
    {
        var teacher = new Teacher("user-1", "Anna");
        var student = new Student("user-2", "Alex");
        Assert.Throws<DomainException>(() => teacher.RemoveStudent(student.Id));
    }
    
    
}