namespace UrlShortener.Application.DTOs;

public record ShortenRequest(
    string Url,
    string? Alias,
    DateTime? ExpiresAt
);

public record ShortenResponse(
    string Code,
    string ShortUrl,
    string OriginalUrl,
    string? Alias,
    long ClickCount,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);

public record ErrorResponse(string Error);
