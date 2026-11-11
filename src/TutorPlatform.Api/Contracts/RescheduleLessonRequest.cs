namespace TutorPlatform.Api.Contracts;

public record RescheduleLessonRequest(DateTime Start, int DurationMinutes);