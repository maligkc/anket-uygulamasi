# Anket Platformu (Survey Platform)

Basit formlar ve anketler oluşturmak, yayınlamak ve yanıtları toplamak için kullanılan web tabanlı bir platform. Google Forms'a benzer bir deneyim sunar.

## 📋 Özellikler

- **Kullanıcı Yönetimi**
  - Kayıt ve giriş (JWT tabanlı kimlik doğrulama)
  - Güvenli oturum yönetimi

- **Form Yönetimi**
  - Form oluşturma, düzenleme, silme
  - Taslak ve yayınlanan durumlar
  - Form yayınlama ve yayından kaldırma
  - Benzersiz paylaşım bağlantıları

- **Soru Türleri**
  - Kısa metin
  - Paragraf
  - Tek seçim (radyo butonları)
  - Çoklu seçim (checkbox)
  - Zorunlu/isteğe bağlı soru ayarları

- **Yanıt Yönetimi**
  - Form sahipleri yanıtları görebilir
  - Seçim sorularının özet istatistikleri
  - Gönderilerin tarih ve saat bilgisi

- **Herkese Açık Erişim**
  - Yayınlanan formlar benzersiz bağlantı ile paylaşılabilir
  - Kimlik doğrulama olmadan yanıt gönderme

## 🛠️ Teknolojiler

### Backend
- **.NET 10** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Veritabanı
- **JWT (JSON Web Tokens)** - Kimlik doğrulama
- **BCrypt** - Şifre hashleme
- **Clean Architecture** - Kod organizasyonu

### Frontend
- **React 18** - UI framework
- **Vite** - Build tool
- **Axios** - HTTP istemcisi
- **React Router** - Yönlendirme
- **JavaScript (ES6+)** - Programlama dili

### DevOps
- **Docker** - Konteynerizasyon (PostgreSQL için)
- **Git** - Versiyon kontrolü

## 📁 Proje Yapısı

```
anket-uygulamasi/
├── backend/
│   └── src/
│       ├── SurveyPlatform.Api/           # Web API ve controller'lar
│       ├── SurveyPlatform.Application/   # İş mantığı ve servisler
│       ├── SurveyPlatform.Domain/        # Varlıklar ve kurallar
│       └── SurveyPlatform.Infrastructure/ # Veritabanı ve dış servisler
├── frontend/
│   └── src/
│       ├── pages/         # Sayfa bileşenleri
│       ├── components/    # Tekrar kullanılabilir bileşenler
│       ├── api/           # API çağrıları
│       ├── hooks/         # React hooks
│       ├── context/       # Durum yönetimi
│       ├── types/         # TypeScript tür tanımları
│       └── utils/         # Yardımcı fonksiyonlar
└── docker-compose.yml     # PostgreSQL konfigürasyonu
```

## ⚙️ Gereksinimler

- **.NET SDK 10** veya üstü
- **Node.js 20+** ve **npm**
- **Docker** (PostgreSQL için isteğe bağlı)
- **PostgreSQL 14+** (yerel kurulum veya Docker)

## 🚀 Kurulum ve Çalıştırma

### 1. Depoyu Klonla

```bash
git clone https://github.com/your-username/anket-uygulamasi.git
cd anket-uygulamasi
```

### 2. PostgreSQL'i Başlat

**Docker ile (önerilir):**
```bash
docker compose up -d
```

**Yerel PostgreSQL ile:**
Bağlantı dizesini `backend/src/SurveyPlatform.Api/appsettings.json` dosyasında güncelleyin.

### 3. Veritabanı Migration'larını Uygula

```bash
cd backend
dotnet ef database update \
  --project src/SurveyPlatform.Infrastructure/SurveyPlatform.Infrastructure.csproj \
  --startup-project src/SurveyPlatform.Api/SurveyPlatform.Api.csproj
```

### 4. Backend'i Çalıştır

```bash
dotnet run --project src/SurveyPlatform.Api/SurveyPlatform.Api.csproj --urls http://localhost:5000
```

Backend erişim: `http://localhost:5000`  
Swagger API Docu: `http://localhost:5000/swagger`

### 5. Frontend'i Çalıştır (yeni terminal)

```bash
cd frontend
npm install
npm run dev
```

Frontend erişim: `http://localhost:5173`

## 📝 Kullanım

1. `http://localhost:5173` adresine gidin
2. **Kayıt ol** sekmesinde yeni bir hesap oluşturun
3. **Giriş yap** sekmesinde hesabınıza giriş yapın
4. **Yeni form oluştur** butonuna tıklayarak form oluşturmaya başlayın
5. Sorular ekleyin ve yapılandırın
6. Formu **Yayınla** ve benzersiz bağlantısı ile paylaşın
7. **Yanıtlar** sekmesinde gönderilen yanıtları görüntüleyin

## 🔐 Güvenlik

- JWT token'ları localStorage'da saklanır (MVP aşaması)
- Şifreler BCrypt ile hashlenir
- E-posta adresleri benzersiz ve gereklidir
- Formlara yalnızca sahipleri erişebilir (herkese açık formlar hariç)

## 📊 API Endpoints

### Kimlik Doğrulama
- `POST /api/auth/register` - Yeni kullanıcı kayıt
- `POST /api/auth/login` - Kullanıcı giriş

### Formlar
- `GET /api/forms` - Kullanıcının formlarını listele
- `POST /api/forms` - Yeni form oluştur
- `GET /api/forms/{id}` - Form detayını al
- `PUT /api/forms/{id}` - Formu güncelle
- `DELETE /api/forms/{id}` - Formu sil
- `POST /api/forms/{id}/publish` - Formu yayınla
- `POST /api/forms/{id}/unpublish` - Formu yayından kaldır

### Sorular
- `POST /api/forms/{id}/questions` - Soru ekle
- `PUT /api/forms/{formId}/questions/{questionId}` - Soruyu güncelle
- `DELETE /api/forms/{formId}/questions/{questionId}` - Soruyu sil

### Herkese Açık Formlar
- `GET /api/public/forms/{shareKey}` - Herkese açık formu al
- `POST /api/public/forms/{shareKey}/responses` - Yanıt gönder

### Yanıtlar
- `GET /api/forms/{id}/responses` - Form yanıtlarını al

## 🏗️ Mimari Kararlar

- **Clean Architecture**: Katmanlı mimari ile sorunlar ayrıştırıldı
- **Modüler Monolitik Yapı**: Monolitik uygulama ancak modülerlik korundu
- **Entity Framework Core**: Güçlü ORM ile veritabanı yönetimi
- **CORS Desteği**: Frontend-backend iletişimi için

## ⚠️ Bilinmeyen Sorunlar

Bazı transitive paketlerdeki güvenlik uyarıları (`NU1903`) var. Üretim ortamında paket zinciri gözden geçirilmelidir.

## 📄 Lisans

MIT Lisansı altında yayınlanmıştır.

## 👤 Geliştirici

Mali
