using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Contracts;

public record ProfileResponse(
    string UserId,
    Guid? ProfileId,
    string? Email,
    string? Name,
    string? Role);