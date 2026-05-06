using UrlShortener.Core.Entities;

namespace UrlShortener.Core.Interfaces;

public interface IUrlShortenerService
{
    Task<ShortUrl> ShortenAsync(string originalUrl, string? alias = null, DateTime? expiresAt = null);
    Task<ShortUrl?> ResolveAsync(string code);
    Task<IEnumerable<ShortUrl>> GetAllAsync();
    Task<bool> DeleteAsync(string code);
}
