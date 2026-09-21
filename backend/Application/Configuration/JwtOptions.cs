namespace Application.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "JWT";

    public string SecretKey { get; init; } = string.Empty;
    public string ValidAudience { get; init; } = string.Empty;
    public string ValidIssuer { get; init; } = string.Empty;
    public double TokenValidityInHours { get; init; } = 8;
    public double RefreshTokenValidityInHours { get; init; } = 24;
}