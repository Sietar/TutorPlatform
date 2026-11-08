using TutorPlatform.Core.Entities;

namespace TutorPlatform.Core.Tests;

public class LessonSchedulerTests
{
    [Fact]
    public void EnsureNoConflicts_WhenLessonRescheduledOverOwnSlot_DoesNotThrow()
    {
        var lesson = CreateLesson(11, 16, 0, 1);
        List<Lesson> lessons = [lesson];

        lesson.Reschedule(lesson.Start.AddMinutes(30), TimeSpan.FromHours(1));

        LessonScheduler.EnsureNoConflicts(lesson, lessons);
    }
    
    [Fact]
    public void EnsureNoConflicts_LessonsCantOverlap_Throws()
    {
        
        var lesson1 = CreateLesson(11, 13, 16, 1);
        var lesson2 = CreateLesson(11, 12, 16, 1.5);
        var lesson3 = CreateLesson(11, 16, 16, 1);

        List<Lesson> lessonList = [lesson2, lesson3];
        
        Assert.Throws<DomainException> (() => LessonScheduler.EnsureNoConflicts(lesson1, lessonList));
    }

    [Fact]
    public void EnsureNoConflicts_ValidLessons_DoesNotThrow()
    {
        var lesson1 = CreateLesson(11, 16, 0, 1);
        var lesson2 = CreateLesson(11, 10, 0, 1);
        var lesson3 = CreateLesson(11, 14, 0, 1);

        List<Lesson> lessonList = [lesson2, lesson3];

        LessonScheduler.EnsureNoConflicts(lesson1, lessonList);
    }

    [Fact]
    public void EnsureNoConflicts_LessonsWithSameTimeButDifferentDate_DoesNotThrow()
    {
        var lesson1 = CreateLesson(11, 16, 0, 1);
        var lesson2 = CreateLesson(12, 16, 0, 1);
        var lesson3 = CreateLesson(13, 16, 0, 1);

        List<Lesson> lessonList = [lesson2, lesson3];

        LessonScheduler.EnsureNoConflicts(lesson1, lessonList);
    }

    [Fact]
    public void EnsureNoConflicts_ConsequentLessons_DoesNotThrow()
    {
        var lesson1 = CreateLesson(11, 16, 0, 1);
        var lesson2 = CreateLesson(11, 17, 0, 1);
        var lesson3 = CreateLesson(11, 18, 0, 1);

        List<Lesson> lessonList = [lesson2, lesson3];

        LessonScheduler.EnsureNoConflicts(lesson1, lessonList);
    }

    [Fact]
    public void EnsureNoConflicts_CanceledLessons_DoesNotThrow()
    {
        var lesson1 = CreateLesson(11, 13, 0, 1);
        var lesson2 = CreateLesson(11, 13, 0, 1);
        var lesson3 = CreateLesson(12, 13, 0, 1);
        
        lesson2.Cancel();

        List<Lesson> lessonList = [lesson2, lesson3];

        LessonScheduler.EnsureNoConflicts(lesson1, lessonList);
    }

    [Theory]
    [InlineData(13, 0, 15, 0)]
    [InlineData(13, 0, 14, 0)]
    [InlineData(13, 0, 12, 0)]
    [InlineData(13, 30, 15, 0)]
    [InlineData(15, 30, 14, 0)]
    public void EnsureNoConflicts_WhenNoOverlapping_DoesNotThrow(int newHours, int newMinutes, int existingHours,
        int existingMinutes)
    {
        var newLesson = CreateLesson(11, newHours, newMinutes, 1);
        var existingLesson = CreateLesson(11, existingHours, existingMinutes, 1);
        
        IEnumerable<Lesson> existingLessonsList = [existingLesson];
        
        LessonScheduler.EnsureNoConflicts(newLesson, existingLessonsList);
    }
    
    [Theory]
    [InlineData(13, 0, 13, 30)]
    [InlineData(13, 0, 12, 30)]
    [InlineData(23, 30, 23, 40)]
    [InlineData(16, 10, 15, 30)]
    [InlineData(17, 45, 18, 0)]
    public void EnsureNoConflicts_WhenOverlapping_Throws(int newHours, int newMinutes, int existingHours,
        int existingMinutes)
    {
        var newLesson = CreateLesson(11, newHours, newMinutes, 1);
        var existingLesson = CreateLesson(11, existingHours, existingMinutes, 1);
        
        IEnumerable<Lesson> existingLessonsList = [existingLesson];
        
        Assert.Throws<DomainException> (() => LessonScheduler.EnsureNoConflicts(newLesson, existingLessonsList));
    }

    [Fact]
    public void EnsureNoConflicts_WhenOverlappingAfterMidnight_Throws()
    {
        var lesson1 = CreateLesson(11, 23, 30, 1);
        var lesson2 = CreateLesson(12, 0, 10, 1);
        var lesson3 = CreateLesson(13, 0, 10, 1);
        
        IEnumerable<Lesson> lessonList = [lesson2, lesson3];
        Assert.Throws<DomainException> (() => LessonScheduler.EnsureNoConflicts(lesson1, lessonList));
    }


    private Lesson CreateLesson(int day, int hour, int minute, double duration)
    {
        return new Lesson(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 11, day, hour, minute, 00, DateTimeKind.Utc), 
            TimeSpan.FromHours(duration));
    }
}