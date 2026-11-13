using TutorPlatform.Core.Entities;
using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Tests;

public class LessonTests
{
    private static readonly DateTime Date = new DateTime(2016, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly Lesson _lesson = new (Guid.CreateVersion7(), Guid.CreateVersion7(), Date, TimeSpan.FromHours(1));

    [Fact]
    public void Constructor_InvalidTeacherId_Throws()
    {
        Assert.Throws<DomainException>(() => new Lesson(Guid.Empty, Guid.CreateVersion7(), Date, TimeSpan.FromHours(1)));
    }
    
    [Fact]
    public void Constructor_InvalidStudentId_Throws()
    {
        Assert.Throws<DomainException>(() => new Lesson(Guid.CreateVersion7(), Guid.Empty, Date, TimeSpan.FromHours(1)));
    }

    [Fact]
    public void Constructor_InvalidDateTimeKind_Throws()
    {
        Assert.Throws<DomainException>(() => new Lesson(Guid.CreateVersion7(),  Guid.CreateVersion7(), Date.ToLocalTime(), TimeSpan.FromHours(1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-202)]
    [InlineData(-43)]
    public void Constructor_InvalidDuration_Throws(int duration)
    {
        Assert.Throws<DomainException>(() => new Lesson(Guid.CreateVersion7(), Guid.CreateVersion7(), Date,
            TimeSpan.FromHours(duration)));
    }

    [Theory]
    [InlineData(30)]
    [InlineData(10)]
    [InlineData(25)]
    public void IsAwaitingMark_WhenNotExceedThirtyMin_ReturnsFalse(int minutes)
    {
        var timeAfterEnd = new DateTime(2016, 1, 1, 13, minutes, 0, DateTimeKind.Utc);
        Assert.False(_lesson.IsAwaitingMark(timeAfterEnd));
    }
    [Theory]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(19)]
    public void IsAwaitingMark_WhenExceedTwoHours_ReturnsTrue(int hours)
    {
        var timeAfterEnd = new DateTime(2016, 1, 1, hours, 01, 0);
        Assert.True(_lesson.IsAwaitingMark(timeAfterEnd));
    }

    [Fact]
    public void IsAwaitingMark_WhenCanceled_ReturnsFalse()
    {
        _lesson.Cancel();
        Assert.False(_lesson.IsAwaitingMark(Date + TimeSpan.FromHours(1.6)));
    }
    
    [Fact]
    public void IsAwaitingMark_WhenCompleted_ReturnsFalse()
    {
        _lesson.Complete();
        Assert.False(_lesson.IsAwaitingMark(Date + TimeSpan.FromHours(1.6)));
    }
    
    
    
    [Fact]
    public void Complete_WhenPlanned_SetsCompleted()
    {
        _lesson.Complete();
        
        Assert.Equal(LessonStatus.Completed, _lesson.Status);
    }
    
    [Fact]
    public void Cancel_WhenPlanned_SetsCanceled()
    {
        _lesson.Cancel();
        
        Assert.Equal(LessonStatus.Canceled, _lesson.Status);
    }

    [Fact]
    public void Complete_WhenCompleted_Throws()
    {
        _lesson.Complete();
        Assert.Throws<DomainException>(_lesson.Complete);
    }
    
    [Fact]
    public void Complete_WhenCanceled_Throws()
    {
        _lesson.Cancel();
        Assert.Throws<DomainException>(_lesson.Complete);
    }
    
    [Fact]
    public void Cancel_WhenCanceled_Throws()
    {
        _lesson.Cancel();
        Assert.Throws<DomainException>(_lesson.Cancel);
    }
    
    [Fact]
    public void Cancel_WhenCompleted_Throws()
    {
        _lesson.Complete();
        Assert.Throws<DomainException>(_lesson.Cancel);
    }

    [Fact]
    public void Reschedule_WhenValid_ChangesDateAndDuration()
    {
        var newDate = Date.AddHours(5);
        _lesson.Reschedule(newDate, TimeSpan.FromHours(2));
        
        Assert.Equal(newDate, _lesson.Start);
        Assert.Equal(TimeSpan.FromHours(2), _lesson.Duration);
    }
    
    [Fact]
    public void Reschedule_WhenCanceled_Throws()
    {
        _lesson.Cancel();
        Assert.Throws<DomainException>(() => _lesson.Reschedule(Date, TimeSpan.FromHours(1.6)));
    }

    [Fact]
    public void Reschedule_WhenCompleted_Throws()
    {
        _lesson.Complete();
        Assert.Throws<DomainException>(() => _lesson.Reschedule(Date, TimeSpan.FromHours(1.6)));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-202)]
    public void Reschedule_WhenInvalidDuration_Throws(int duration)
    {
        Assert.Throws<DomainException>(() => _lesson.Reschedule(Date, TimeSpan.FromHours(duration)));
    }

    [Fact]
    public void Reschedule_WhenInvalidDateTimeKind_Throws()
    {
        Assert.Throws<DomainException>(() => _lesson.Reschedule(Date.ToLocalTime(), TimeSpan.FromHours(-1)));
    }
}