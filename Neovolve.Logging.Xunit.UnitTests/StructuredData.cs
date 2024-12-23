namespace Neovolve.Logging.Xunit.UnitTests;

using System;

// ReSharper disable once ClassNeverInstantiated.Local
// ReSharper disable UnusedMember.Local
public class StructuredData
{
    public static StructuredData BuildData()
    {
        return new StructuredData
        {
            DateOfBirth = DateTime.UtcNow,
            Email = Guid.NewGuid().ToString(),
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
        };
    }

    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// ReSharper restore once ClassNeverInstantiated.Local
// ReSharper restore UnusedMember.Local