namespace TutorPlatform.Api.Contracts;

public record CreateHomeworkRequest(Guid TeacherId, Guid StudentId, string Title, string? Description, DateTime DueDate, string? MaterialsUrl = null);