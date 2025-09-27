# Flappy Bird Oyunu (C# Windows Forms)

Bu proje, popüler Flappy Bird oyununun C# Windows Forms kullanılarak geliştirilmiş bir versiyonudur. Oyun, ilerledikçe zorluk seviyesi artan, gece/gündüz döngüsü içeren ve Angry Birds temalı görsellere sahip basit bir 2D platform oyunudur.

## Özellikler

*   **Oyun Mekanikleri:** Kuşun zıplaması, yer çekimi, borulardan geçiş, çarpışma algılama.
*   **Görseller:** Angry Birds temalı kuş (`redbird-midflap.png`), borular (`pipe-red.png`), zemin (`base.png`) ve iki farklı arka plan (`background-day.png`, `background.png` - gece için).
*   **Kullanıcı Arayüzü:** Başlangıç mesajı (`message.png`), oyun bitti ekranı (`gameover.png`) ve skor görselleri (`0.png` - `9.png`).
*   **Zorluk Artışı:** Oyun ilerledikçe boruların hızı artar ve aralarındaki boşluk daralır.
*   **Gece/Gündüz Döngüsü:** Belirli skor aralıklarında oyunun arka planı gece ve gündüz arasında geçiş yapar.
*   **Tam Ekran Modu:** Uygulama başlatıldığında tam ekran olarak açılır.
*   **Kontroller:**
    *   `Boşluk Tuşu (Spacebar)`: Kuşu zıplatır ve oyunu başlatır.
    *   `R Tuşu`: Oyun bittiğinde yeniden başlatır.
    *   `Escape Tuşu (Esc)`: Uygulamayı kapatır.

## Kurulum ve Çalıştırma

1.  **Visual Studio Yüklemesi:** Bilgisayarınızda Microsoft Visual Studio'nun yüklü olduğundan emin olun (Community sürümü ücretsizdir). Kurulum sırasında ".NET ile Masaüstü geliştirmesi" iş yükünü seçtiğinizden emin olun.
2.  **Projeyi Klonlama/İndirme:** Bu depoyu GitHub'dan klonlayın veya zip olarak indirin.
3.  **Visual Studio'da Açma:** Proje klasöründeki `FlappyBirdGame.sln` dosyasını çift tıklayarak Visual Studio'da açın.
4.  **Görsel Varlıkları Ekleme (Eğer projenizde yoksa):**
    *   Bu projede kullanılan görseller, `Assets` klasöründe yer almaktadır. Bu görsellerin `Build Action` özelliğinin `Content` ve `Copy to Output Directory` özelliğinin `Copy if newer` olarak ayarlandığından emin olun.
5.  **Form Ayarlarını Kontrol Etme:**
    *   `Form1.cs [Tasarım]` sekmesinde Form'u seçin.
    *   `Properties` (Özellikler) penceresinde `FormBorderStyle` özelliğini `Sizable` ve `WindowState` özelliğini `Normal` olarak ayarlayın.
6.  **Projeyi Derleme ve Çalıştırma:**
    *   Visual Studio'da üst menüden `Hata Ayıkla (Debug) -> Hata Ayıklamayı Başlat (Start Debugging)` seçeneğini (veya `F5` tuşunu) tıklayın.

## Kullanılan Görseller

Tüm görseller `Assets` klasöründe bulunmaktadır:

*   `0.png` - `9.png` (Skor sayıları)
*   `background.png` (Gece arka planı)
*   `background-day.png` (Gündüz arka planı)
*   `base.png` (Zemin)
*   `gameover.png` (Oyun bitti mesajı)
*   `message.png` (Başlangıç mesajı)
*   `pipe-red.png` (Borular)
*   `redbird-midflap.png` (Kuş)

## Katkıda Bulunma

Geliştirmelere veya iyileştirmelere açık bir projedir. Her türlü katkı (kod, görsel, hata düzeltmesi) memnuniyetle karşılanır.

---
