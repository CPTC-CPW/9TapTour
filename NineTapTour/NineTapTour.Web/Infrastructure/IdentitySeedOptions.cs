namespace NineTapTour.Web.Infrastructure;

/// <summary>
/// First admin account, bound from the "Identity:Admin" configuration section.
/// The password is never committed: set it with user-secrets locally
/// (dotnet user-secrets set "Identity:Admin:Password" "...") or the
/// Identity__Admin__Password environment variable on the host.
/// </summary>
public sealed class IdentitySeedOptions
{
    public const string SectionName = "Identity:Admin";

    public string UserName { get; set; } = "admin";

    public string? Password { get; set; }
}
