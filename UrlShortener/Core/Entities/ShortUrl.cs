namespace UrlShortener.Core.Entities;

public class ShortUrl
{
    public int Id { get; set; }
    public required string Code { get; set; }       // e.g. "aB3kZx"
    public required string OriginalUrl { get; set; }
    public string? Alias { get; set; }              // custom alias (optional)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }        // null = never expires
    public long ClickCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
