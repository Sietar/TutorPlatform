namespace TutorPlatform.Core.Tests;

public class LessonTests
{
    /*Write tests for overlapping lessons, that canceled lessons don't block anything and that two consequential lessons possible
     (one ends at 15:00 and the next one starts at 15:00). 
     
     Assure that lessons statuses change correctly (Canceled can't become completed or planned, completed can't be canceled or planned)
     
     */

    [Fact]
    public void AddLesson_LessonsCantOverlap_Throws()
    {
        
    }
}