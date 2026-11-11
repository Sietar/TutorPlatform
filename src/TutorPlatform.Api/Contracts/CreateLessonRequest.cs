namespace TutorPlatform.Api.Contracts;

public record CreateLessonRequest(Guid TeacherId, Guid StudentId, DateTime Start, int DurationMinutes);