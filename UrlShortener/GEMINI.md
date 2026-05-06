# Project: SNIP — URL Shortener

Bu proje, ASP.NET Core 8 Minimal API ve SQLite kullanılarak geliştirilmiş modern bir URL kısaltma servisidir.

## 🛠 Teknik Yığın (Tech Stack)

- **Backend:** .NET 8 (Minimal APIs)
- **Database:** SQLite (Entity Framework Core)
- **Frontend:** Vanilla JavaScript, CSS, HTML
- **Containerization:** Docker & Docker Compose

## 📁 Proje Yapısı

- `Program.cs`: API rotaları, middleware konfigürasyonu ve DI ayarlarını içerir.
- `Data/`: DBContext (`AppDbContext`) tanımları.
- `Models/`: Domain modelleri (`ShortUrl`) ve DTO'lar (`Dtos.cs`).
- `Services/`: İş mantığı (`UrlShortenerService`).
- `wwwroot/`: Frontend dosyaları.

## 📜 Mimari Kararlar ve Kurallar

- **Base62 Kodlama:** Rastgele linkler için 6 karakterli Base62 alfabesi kullanılır.
- **Deduplication:** Aynı URL tekrar kısaltılmak istendiğinde (alias yoksa) mevcut kayıt döndürülür.
- **Soft Delete:** Linkler `IsActive = false` yapılarak silinmiş sayılır, veritabanından tamamen kaldırılmaz.
- **Validation:** URL'ler `http` veya `https` ile başlamalıdır.
- **Frontend:** Herhangi bir framework kullanılmadan "vanilla" yaklaşımla geliştirilmiştir.

## 🚀 Önemli Komutlar

- **Local Çalıştırma:** `dotnet run`
- **Docker Build:** `docker build -t url-shortener .`
- **Docker Compose:** `docker compose up -d`
- **Yeni Migration Ekleme:** `dotnet ef migrations add <Name>`

## 💡 Geliştirici Notları

- SQLite veritabanı `urls.db` adıyla proje kökünde veya Docker'da `/app/data/urls.db` yolunda tutulur.
- Yeni bir endpoint eklerken `Program.cs` içindeki Minimal API pattern'i takip edilmelidir.
- İş mantığı her zaman `UrlShortenerService` içinde kalmalı, Controller (veya MapPost/Get) seviyesinde sadece request/response yönetilmelidir.
