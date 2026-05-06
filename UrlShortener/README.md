# SNIP — URL Shortener

ASP.NET Core 8 Minimal API + SQLite + vanilla JS frontend.

## Özellikler

- Base62 random code üretimi (6 karakter = ~56 milyar kombinasyon)
- Custom alias desteği
- Link süresi (ExpiresAt)
- Click count takibi
- Aynı URL tekrar kısaltılırsa deduplicate eder
- Redirect sırasında expired kontrolü

## Çalıştırma

### Local (dotnet CLI)
```bash
dotnet run
# → http://localhost:5000
```

### Docker
```bash
docker compose up -d
# → http://localhost:8080
```

## API

### POST /api/shorten
```json
{
  "url": "https://example.com/uzun/link",
  "alias": "benim-link",        // opsiyonel
  "expiresAt": "2025-12-31T00:00:00Z"  // opsiyonel
}
```

**Response:**
```json
{
  "code": "aB3kZx",
  "shortUrl": "http://localhost:8080/aB3kZx",
  "originalUrl": "https://example.com/uzun/link",
  "alias": null,
  "clickCount": 0,
  "createdAt": "2025-01-01T00:00:00Z",
  "expiresAt": null
}
```

### GET /{code}
→ 302 redirect to original URL
→ click count++

### GET /api/links
Tüm aktif linklerin listesi.

### DELETE /api/links/{code}
Soft delete (IsActive = false).

## Production Notları

- SQLite yeterlidir düşük-orta yük için.
  Yüksek yük → PostgreSQL için EF Core provider'ını swap et, 1 satır değişir.
- Distributed cache (Redis) ile redirect path'ini cache'leyebilirsin.
- Rate limiting için ASP.NET Core'un built-in `AddRateLimiter` middleware'ini ekle.
- Base URL env variable'dan al: `appsettings.Production.json`'a `"BaseUrl": "https://snip.domain.com"` ekle.
