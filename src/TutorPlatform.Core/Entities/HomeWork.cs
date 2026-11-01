using TutorPlatform.Core.Enums;

namespace TutorPlatform.Core.Entities;

public class HomeWork
{
    public HomeWorkStatus  Status { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime DueTo { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
}