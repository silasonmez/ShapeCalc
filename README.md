# 📐 ShapeCalc – Çok Katmanlı Şekil Hesaplama Uygulaması

ShapeCalc, kullanıcıların DevExpress GridView arayüzü üzerinden kare, dikdörtgen ve üçgen gibi geometrik şekil bilgilerini girmesini ve bu bilgilerin REST API’ler aracılığıyla hesaplanarak veritabanına kaydedilmesini sağlayan çok katmanlı bir ASP.NET projesidir.

---

## 🧩 Proje Yapısı

- **DXApplication1 (Portal)**: Kullanıcının GridView üzerinden giriş yaptığı DevExpress arayüzü (ASP.NET MVC).
- **InputApi**: API1. Giriş verilerini `IsCalculated = false` filtreleyerek paketler ve ComputeApi’ye iletir.
- **ComputeApi**: API2. Gelen şekil türüne göre hesaplama yapar ve sonuçları veritabanına kaydeder.

---

## 🧮 Desteklenen Şekiller

- ✅ Kare → Alan
- ✅ Dikdörtgen → Alan
- ✅ Üçgen → Alan
- (Yeni şekiller kolayca entegre edilebilir yapıdadır.)

---

## 🔄 Veri Akışı

1. Kullanıcı Portal arayüzünden şekil bilgisi girer.
2. InputApi, bu verileri `IsCalculated = false` filtresiyle alır ve uygun paketlere böler.
3. ComputeApi, gelen verileri türüne göre işler (örneğin kare → alan = kenar²).
4. Hesaplanan sonuçlar `IsCalculated = true` olarak işaretlenir ve `AreaOrVolume` alanına yazılır.

---

## 💡 Kullanılan Teknolojiler

- ASP.NET MVC (.NET Framework)
- DevExpress GridView
- Web API (REST)
- Entity Framework
- SQL Server
- Postman (test için)
- Git & GitHub

---

## ⚙️ Kurulum ve Çalıştırma

1. Visual Studio'da tüm projeleri tek solution içinde açın.
2. NuGet bağımlılıklarını yükleyin (EF, DevExpress vb.).
3. `ComputeApi` ve `InputApi` projelerini farklı portlarda çalıştırın.
4. `DXApplication1` (Portal) projesini başlatın.
5. GridView üzerinden veri girin, ardından "Hesapla" butonuna tıklayın.
6. Sonuçlar arka planda hesaplanıp GridView'e yansıtılacaktır.

---

## 🌐 Çoklu Port Mimarisi (Multi-API Çalıştırma)

Proje geliştirici ortamında birden fazla portta çalışan bağımsız servislerden oluşur. Her servis farklı portlarda ayağa kalkar ve aralarında veri akışı REST API'ler üzerinden gerçekleşir.

### 🔗 Kullanılan Portlar

- 🟢 `InputApi` → http://localhost:7001 (Visual Studio üzerinden başlatılır)
- 🔵 `ComputeApi` → http://localhost:7002 (PowerShell ile manuel başlatılır)
- 🟠 `DXApplication1` (Portal arayüzü) → http://localhost:63563 (Visual Studio’dan başlatılır)

### 🛠️ ComputeApi’yi manuel çalıştırmak için:

```powershell
Start-Process "C:\Program Files (x86)\IIS Express\iisexpress.exe" -ArgumentList '/path:"C:\Users\silas\Desktop\staj25\portal\ComputeApi"', '/port:7002'
```

> Bu sayede `InputApi`, Portal'dan gelen istekleri `ComputeApi`'ye iletebilir. Her servis bağımsız portlarda çalıştığı için, gerçekçi bir çok katmanlı mimari elde edilir.

---

## 🧪 **Test Süreci**

- ✅ **Kullanıcı giriş ve yetkilendirme işlemleri**
- ✅ **GridView üzerinden CRUD testleri**
- ✅ **API çağrıları Postman ile test edildi**
- ✅ **InputApi → ComputeApi veri akışı başarıyla doğrulandı**

---

## 📦 **Veritabanı Kolonları (örnek)**


<img width="609" height="284" alt="Ekran görüntüsü 2025-08-07 165111" src="https://github.com/user-attachments/assets/c865f081-e54c-4d81-9b7d-2506bd446323" />




> ✅ Bu tablo `ComputeApi` tarafından doldurulan ve `DXApplication1` üzerinden görüntülenen temel veri yapısını temsil eder.

---

## 🧠 Öğrenilenler

- DevExpress GridView ile inline form geliştirme
- ASP.NET Identity ile kullanıcı yönetimi
- TempData ve ViewBag kullanımı
- Çok katmanlı (UI/API1/API2) sistem tasarımı
- REST mimarisi ile veri paylaşımı
- Gerçek zamanlı hesaplama akışı kurulumu
- Git ve GitHub yönetimi

---

## 👥 Hedef Kullanıcı Kitlesi

- Ortaokul & lise düzeyindeki öğrenciler
- Matematik öğretmenleri
- Mühendislik öğrencileri
- Teknik çizim ve analiz yapan kullanıcılar

---

## 🔗 Projeyi İncele

👉 (https://github.com/silasonmez/ShapeCalc.git)
