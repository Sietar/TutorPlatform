using TutorPlatform.Core.Entities;

namespace TutorPlatform.Core.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_WhenValid_CreatesStudent()
    {
        var student = new Student("user-1", "Anna");
        
        Assert.Null(student.Teacher);
        Assert.NotEqual(Guid.Empty, student.Id);
        Assert.Equal("Anna", student.Name);
        Assert.Equal("user-1", student.UserId);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WhenNameInvalid_Throws(string? name)
    {
        Assert.Throws<DomainException> (() => new Student("user-1",  name!));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WhenUserIdInvalid_Throws(string? id)
    {
        Assert.Throws<DomainException> (() => new Student(id!,  "Anna"));
    }
}