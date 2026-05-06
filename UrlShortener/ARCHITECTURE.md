# Project Architecture: SNIP — URL Shortener

Bu proje, mantıksal bir N-tier (Çok Katmanlı) mimari kullanılarak organize edilmiştir. Tek bir `.csproj` içerisinde klasörler bazında sorumluluklar ayrılmıştır.

## 📂 Klasör Yapısı ve Sorumluluklar

### 1. Core (Çekirdek Katmanı)
Uygulamanın temel iş mantığından bağımsız olan ve tüm katmanlar tarafından paylaşılan bileşenleri içerir.
- **Entities:** Veritabanı tablolarını temsil eden domain modelleri (`ShortUrl`).
- **Interfaces:** Servislerin kontratları (`IUrlShortenerService`). Bu katman, bağımlılıkların tersine çevrilmesini (Dependency Inversion) sağlar.

### 2. Application (Uygulama Katmanı)
İş mantığının (Business Logic) yürütüldüğü katmandır.
- **Services:** İş mantığını gerçekleştiren sınıflar (`UrlShortenerService`).
- **DTOs:** Veri transfer nesneleri (`ShortenRequest`, `ShortenResponse`). API ve istemci arasında veri taşımak için kullanılır.

### 3. Infrastructure (Altyapı Katmanı)
Dış kaynaklara (veritabanı, dosya sistemi, dış servisler) erişim sağlayan katmandır.
- **Data:** Entity Framework Core `DbContext` ve veritabanı yapılandırmaları.

### 4. Presentation (Sunum/API Katmanı) - `Program.cs`
İstemciden gelen isteklerin karşılandığı ve yanıtların döndürüldüğü giriş noktasıdır.
- Minimal API route tanımları.
- Middleware konfigürasyonu.
- Dependency Injection (DI) kayıtları.

## 🛠 Bağımlılık Akışı (Dependency Flow)

Bağımlılıklar her zaman içe (Core'a) doğrudur:
`Presentation` → `Application` → `Core`
`Infrastructure` → `Core`

`Presentation` katmanı hem `Application` hem de `Infrastructure` katmanlarını kullanarak uygulamayı ayağa kaldırır.

## ✅ Mimari Avantajlar
- **Bakım Kolaylığı:** Kodun nerede olduğu bellidir (Seperation of Concerns).
- **Test Edilebilirlik:** Arayüzler sayesinde iş mantığı kolayca birim testlerine tabi tutulabilir.
- **Genişletilebilirlik:** Veritabanı veya servis değişimleri diğer katmanları etkilemeden yapılabilir.
