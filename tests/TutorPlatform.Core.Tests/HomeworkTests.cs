using TutorPlatform.Core.Entities;
using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Tests;

public class HomeworkTests
{
    private readonly DateTime _startDate = new DateTime(2026, 11, 11, 11, 11, 11, DateTimeKind.Utc);
    private readonly DateTime _dueDate = new DateTime(2026, 11, 16, 11, 11, 11, DateTimeKind.Utc);

    [Fact]
    public void Constructor_WhenValid_DoesntThrow()
    {
        CreateValidHomework();
    }
    
    [Fact]
    public void Constructor_WhenTeacherIdEmpty_Throws()
    {
        Assert.Throws<DomainException>(() => new Homework(Guid.Empty, Guid.CreateVersion7(), "Title", "Description", _startDate, _dueDate ));
    }
    
    [Fact]
    public void Constructor_WhenStudentIdEmpty_Throws()
    {
        Assert.Throws<DomainException>(() => new Homework(Guid.CreateVersion7(), Guid.Empty, "Title", "Description", _startDate, _dueDate ));
    }
    
    [Fact]
    public void Constructor_WhenTitleEmpty_Throws()
    {
        Assert.Throws<DomainException>(() => new Homework(Guid.CreateVersion7(), Guid.CreateVersion7(), "", "Description", _startDate, _dueDate ));
    }

    [Fact]
    public void Constructor_WhenDateTimeKindNotUtc_Throws()
    {
        Assert.Throws<DomainException>(() => new Homework(Guid.CreateVersion7(), Guid.CreateVersion7(), "Title", "Description", _startDate.ToLocalTime(), _dueDate ));
    }
    
    [Fact]
    public void Constructor_WhenCreatedAfterDueToDate_Throws()
    {
        Assert.Throws<DomainException>(() => new Homework(Guid.CreateVersion7(), Guid.CreateVersion7(), "Title", "Description", _dueDate, _startDate ));
    }

    [Fact]
    public void IsOverdue_WhenOverdue_ReturnsTrue()
    {
        var homework = CreateValidHomework();
        Assert.True(homework.IsOverdue(new DateTime(2026, 11, 17, 11, 11, 11, DateTimeKind.Utc)));
    }

    [Fact]
    public void IsOverdue_WhenOverdueForCompleted_ReturnsFalse()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        Assert.False(homework.IsOverdue(new DateTime(2026, 11, 17, 11, 11, 11, DateTimeKind.Utc)));
    }

    [Fact]
    public void Submit_WhenValid_Changes()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        
        Assert.Equal(HomeworkStatus.Submitted, homework.Status);
    }

    [Fact]
    public void Submit_WhenReturned_DoesntThrow()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Return("comment");
        homework.Submit("answer");
        
        Assert.Equal(HomeworkStatus.Submitted, homework.Status);
    }

    [Fact]
    public void Submit_WhenEmptyAnswer_Throws()
    {
        var homework = CreateValidHomework();
        Assert.Throws<DomainException>(() => homework.Submit(""));
    }

    [Fact]
    public void Submit_WhenSubmitted_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        Assert.Throws<DomainException>(() => homework.Submit("answer"));
    }
    
    [Fact]
    public void Submit_WhenCanceled_Throws()
    {
        var homework = CreateValidHomework();
        homework.Cancel();
        Assert.Throws<DomainException>(() => homework.Submit("answer"));
    }

    [Fact]
    public void Submit_WhenCompleted_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        Assert.Throws<DomainException>(() => homework.Submit("answer"));
    }

    [Fact]
    public void Return_WhenSubmitted_DoesntThrow()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Return("comment");
        
        Assert.Equal(HomeworkStatus.Returned, homework.Status);
    }

    [Fact]
    public void Return_WhenCanceled_Throws()
    {
        var homework = CreateValidHomework();
        homework.Cancel();
        Assert.Throws<DomainException>(() => homework.Return("comment"));
    }
    
    [Fact]
    public void Return_WhenCompleted_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        Assert.Throws<DomainException>(() => homework.Return("comment"));
    }

    [Fact]
    public void Return_WhenReturned_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Return("comment");
        Assert.Throws<DomainException>(() => homework.Return("comment"));
    }

    [Fact]
    public void Complete_WhenSubmitted_DoesntThrow()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        
        Assert.Equal(HomeworkStatus.Completed, homework.Status);
    }

    [Fact]
    public void Complete_WhenNotSubmitted_Throws()
    {
        var homework = CreateValidHomework();
        Assert.Throws<DomainException>(() => homework.Complete());
    }

    [Fact]
    public void Complete_WhenCanceled_Throws()
    {
        var homework = CreateValidHomework();
        homework.Cancel();
        Assert.Throws<DomainException>(() => homework.Complete());
    }

    [Fact]
    public void Complete_WhenCompleted_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        Assert.Throws<DomainException>(() => homework.Complete());
    }

    [Fact]
    public void Complete_WhenReturned_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Return("comment");
        Assert.Throws<DomainException>(() => homework.Complete());
    }

    [Fact]
    public void Cancel_WhenCanceled_Throws()
    {
        var homework = CreateValidHomework();
        homework.Cancel();
        Assert.Throws<DomainException>(() => homework.Cancel());
    }

    [Fact]
    public void Cancel_WhenCompleted_Throws()
    {
        var homework = CreateValidHomework();
        homework.Submit("answer");
        homework.Complete();
        Assert.Throws<DomainException>(() => homework.Cancel());
    }
    

    private Homework CreateValidHomework()
    {
        return  new Homework(Guid.NewGuid(), Guid.NewGuid(), "Title", "Description", _startDate, _dueDate);
    }
}