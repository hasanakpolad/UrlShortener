using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Application.Services;

public class UrlShortenerService(AppDbContext db) : IUrlShortenerService
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const int CodeLength = 6;

    public async Task<ShortUrl> ShortenAsync(string originalUrl, string? alias = null, DateTime? expiresAt = null)
    {
        // Validate URL
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Geçersiz URL. http veya https ile başlamalı.");

        // Check if alias is taken
        if (!string.IsNullOrWhiteSpace(alias))
        {
            var aliasExists = await db.ShortUrls
                .AsNoTracking()
                .AnyAsync(u => u.Alias == alias || u.Code == alias);

            if (aliasExists)
                throw new InvalidOperationException($"'{alias}' alias zaten kullanımda.");
        }

        // Check if the same URL was already shortened (dedup)
        var existing = await db.ShortUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl && u.IsActive && u.Alias == null);

        if (existing is not null && string.IsNullOrWhiteSpace(alias))
            return existing;

        var code = string.IsNullOrWhiteSpace(alias)
            ? await GenerateUniqueCodeAsync()
            : alias.Trim();

        var shortUrl = new ShortUrl
        {
            Code = code,
            OriginalUrl = originalUrl,
            Alias = string.IsNullOrWhiteSpace(alias) ? null : alias.Trim(),
            ExpiresAt = expiresAt
        };

        db.ShortUrls.Add(shortUrl);
        await db.SaveChangesAsync();
        return shortUrl;
    }

    public async Task<ShortUrl?> ResolveAsync(string code)
    {
        var entry = await db.ShortUrls
            .FirstOrDefaultAsync(u => (u.Code == code || u.Alias == code) && u.IsActive);

        if (entry is null) return null;

        // Expired?
        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
        {
            entry.IsActive = false;
            await db.SaveChangesAsync();
            return null;
        }

        entry.ClickCount++;
        await db.SaveChangesAsync();
        return entry;
    }

    public async Task<IEnumerable<ShortUrl>> GetAllAsync() =>
        await db.ShortUrls
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

    public async Task<bool> DeleteAsync(string code)
    {
        var entry = await db.ShortUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (entry is null) return false;
        entry.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }

    // --- helpers ---

    private async Task<string> GenerateUniqueCodeAsync()
    {
        string code;
        do
        {
            code = GenerateCode();
        } while (await db.ShortUrls.AsNoTracking().AnyAsync(u => u.Code == code));

        return code;
    }

    private static string GenerateCode()
    {
        var chars = new char[CodeLength];
        var bytes = new byte[CodeLength];
        Random.Shared.NextBytes(bytes);

        for (int i = 0; i < CodeLength; i++)
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];

        return new string(chars);
    }
}
