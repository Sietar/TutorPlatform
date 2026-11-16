using TutorPlatform.Core.Entities;

namespace TutorPlatform.Api.Contracts;

public record StudentResponse(Guid Id, string Name)
{
    public static StudentResponse From(Student student)
    {
        return new StudentResponse(student.Id, student.Name);
    }
}