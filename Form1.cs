using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FlappyBirdGame
{
    public enum GameState
    {
        Ready, // Oyun başlamadan önceki durum (başlangıç mesajı)
        Playing, // Oyun oynanıyor
        GameOver // Oyun bitti
    }

    public partial class Form1 : Form
    {
        // Kuş ile ilgili değişkenler
        int birdX = 100; // Kuşun X konumu
        int birdY = 150; // Kuşun Y konumu
        int birdWidth = 45; // Kuşun genişliği (görsel için ayarlandı)
        int birdHeight = 35; // Kuşun yüksekliği (görsel için ayarlandı)
        int birdGravity = 1; // Kuşa etki eden yer çekimi gücü (Azaltıldı: 2 -> 1, daha yumuşak düşüş)
        int birdSpeed = 0; // Kuşun dikey hızı (zıplama için kullanılacak)

        // Boru ile ilgili değişkenler
        List<Rectangle> pipes = new List<Rectangle>(); // Boruların listesi
        int initialPipeSpeed = 5; // Boruların başlangıç hareket hızı (YENİ!)
        int pipeSpeed; // Mevcut boru hızı (oyun ilerledikçe değişecek) (YENİ!)
        int pipeWidth = 70; // Boruların genişliği (görsel yüklendiğinde ayarlanacak)
        int initialPipeGap = 180; // Borular arasındaki başlangıç boşluk (YENİ!)
        int pipeGap; // Mevcut boru boşluğu (oyun ilerledikçe değişecek) (YENİ!)
        int distanceBetweenPipes = 300; // İki boru seti arasındaki yatay mesafe
        Random rand = new Random(); // Boru yüksekliklerini rastgele belirlemek için

        // Oyun durumu değişkenleri
        GameState currentState = GameState.Ready; // Oyun başlangıçta "Hazır" konumunda
        int score = 0;
        int difficultyIncreaseScoreInterval = 10; // Her 10 skorda bir zorluk artsın (YENİ!)


        // Görsel varlıklar
        Image birdImage;
        Image pipeImage; // Boru görseli
        Image backgroundDayImage; // Gündüz arka plan görseli (background-day.png)
        Image backgroundNightImage; // Gece arka plan görseli (background.png)
        Image currentBackgroundImage; // Hangi arka planın aktif olduğunu tutar
        Image baseImage; // Zemin görseli
        Image gameOverImage; // Game Over görseli
        Image messageImage; // Başlangıç mesajı görseli (message.png)
        Image[] scoreNumbers = new Image[10]; // Sayı görselleri için dizi


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyDown += Form1_KeyDown;

            gameTimer.Tick += gameTimer_Tick;
            gameCanvas.Paint += gameCanvas_Paint;

            // Oyun başlangıcında boru hızı ve boşluğunu ayarla
            pipeSpeed = initialPipeSpeed;
            pipeGap = initialPipeGap;


            // Görselleri yükle
            try
            {
                birdImage = Image.FromFile("Assets/redbird-midflap.png");
                pipeImage = Image.FromFile("Assets/pipe-red.png");
                backgroundDayImage = Image.FromFile("Assets/background-day.png");
                backgroundNightImage = Image.FromFile("Assets/background.png");
                baseImage = Image.FromFile("Assets/base.png");
                gameOverImage = Image.FromFile("Assets/gameover.png");
                messageImage = Image.FromFile("Assets/message.png");

                // Oyun başlangıcında gündüz arka planını ayarla
                currentBackgroundImage = backgroundDayImage;

                // Sayı görsellerini yükle
                for (int i = 0; i < 10; i++)
                {
                    scoreNumbers[i] = Image.FromFile($"Assets/{i}.png");
                }

                if (pipeImage != null)
                {
                    pipeWidth = pipeImage.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görsel yüklenirken bir hata oluştu: " + ex.Message + "\nDosya yollarını ve adlarını kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (currentState != GameState.Playing)
            {
                gameCanvas.Invalidate();
                return;
            }

            // Kuşun Y konumunu güncelle (hız ve yer çekimi ile)
            birdSpeed += birdGravity;
            birdY += birdSpeed;

            // Kuşun ekranın dışına çıkmasını engelle (üst sınırlar)
            if (birdY < 0)
            {
                birdY = 0;
                birdSpeed = 0;
            }

            // Zemin çarpışması kontrolü
            int actualBaseHeight = (baseImage != null) ? baseImage.Height : 50;
            if (birdY + birdHeight > gameCanvas.Height - actualBaseHeight)
            {
                birdY = gameCanvas.Height - birdHeight - actualBaseHeight;
                GameOver();
                return;
            }
            else if (baseImage == null && birdY + birdHeight > gameCanvas.Height)
            {
                birdY = gameCanvas.Height - birdHeight;
                GameOver();
                return;
            }

            // Boruların hareketini güncelle
            for (int i = 0; i < pipes.Count; i++)
            {
                pipes[i] = new Rectangle(pipes[i].X - pipeSpeed, pipes[i].Y, pipes[i].Width, pipes[i].Height);
            }

            // Ekrandan çıkan boruları kaldır ve yenilerini ekle
            if (pipes.Count > 0 && pipes[0].Right < -100)
            {
                pipes.RemoveAt(0);
                pipes.RemoveAt(0);
                GenerateNewPipe();
            }

            // Çarpışma kontrolü
            CheckCollisions();

            // Skor kontrolü
            UpdateScore();

            gameCanvas.Invalidate();
        }

        private void gameCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Arka planı çiz (güncel arka plan görselini kullan)
            if (currentBackgroundImage != null)
            {
                g.DrawImage(currentBackgroundImage, 0, 0, gameCanvas.Width, gameCanvas.Height);
            }
            else
            {
                g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, gameCanvas.Width, gameCanvas.Height);
            }

            // Boruları çiz (sadece Playing durumunda)
            if (currentState == GameState.Playing || currentState == GameState.GameOver)
            {
                if (pipeImage != null)
                {
                    foreach (Rectangle pipe in pipes)
                    {
                        if (pipe.Y < 0) // Üst boru
                        {
                            g.TranslateTransform(pipe.X + pipe.Width / 2, pipe.Y + pipe.Height / 2);
                            g.RotateTransform(180);
                            g.DrawImage(pipeImage, -pipe.Width / 2, -pipe.Height / 2, pipe.Width, pipe.Height);
                            g.ResetTransform();
                        }
                        else // Alt boru
                        {
                            g.DrawImage(pipeImage, pipe.X, pipe.Y, pipe.Width, pipe.Height);
                        }
                    }
                }
                else
                {
                    foreach (Rectangle pipe in pipes)
                    {
                        g.FillRectangle(Brushes.Green, pipe);
                    }
                }
            }


            // Zemini çiz
            if (baseImage != null)
            {
                for (int x = 0; x < gameCanvas.Width; x += baseImage.Width)
                {
                    g.DrawImage(baseImage, x, gameCanvas.Height - baseImage.Height, baseImage.Width, baseImage.Height);
                }
            }
            else
            {
                g.FillRectangle(Brushes.Green, 0, gameCanvas.Height - 50, gameCanvas.Width, 50);
            }

            // Kuşu çiz
            if (birdImage != null)
            {
                g.DrawImage(birdImage, birdX, birdY, birdWidth, birdHeight);
            }
            else
            {
                g.FillRectangle(Brushes.Red, birdX, birdY, birdWidth, birdHeight);
            }

            // Oyun durumuna göre çizimler
            if (currentState == GameState.Ready)
            {
                // Başlangıç mesajını çiz
                if (messageImage != null)
                {
                    float messageX = (gameCanvas.Width - messageImage.Width) / 2;
                    float messageY = (gameCanvas.Height - messageImage.Height) / 2 - 50;
                    g.DrawImage(messageImage, messageX, messageY, messageImage.Width, messageImage.Height);
                }
                else
                {
                    Font startFont = new Font("Arial", 36, FontStyle.Bold);
                    string startText = "BAŞLAMAK İÇİN BOŞLUK TUŞUNA BASIN";
                    SizeF textSize = g.MeasureString(startText, startFont);
                    float textX = (gameCanvas.Width - textSize.Width) / 2;
                    float textY = (gameCanvas.Height - textSize.Height) / 2;
                    g.DrawString(startText, startFont, Brushes.Black, textX, textY);
                }
            }
            else if (currentState == GameState.Playing)
            {
                // Skoru çiz
                DrawScoreWithImages(g);
            }
            else if (currentState == GameState.GameOver)
            {
                // Game Over görselini çiz
                if (gameOverImage != null)
                {
                    float gameOverX = (gameCanvas.Width - gameOverImage.Width) / 2;
                    float gameOverY = (gameCanvas.Height - gameOverImage.Height) / 2 - 50;
                    g.DrawImage(gameOverImage, gameOverX, gameOverY, gameOverImage.Width, gameOverImage.Height);
                }
                else
                {
                    Font gameOverFont = new Font("Arial", 24, FontStyle.Bold);
                    string gameOverText = "OYUN BİTTİ!";
                    SizeF textSize = g.MeasureString(gameOverText, gameOverFont);
                    float textX = (gameCanvas.Width - textSize.Width) / 2;
                    float textY = (gameCanvas.Height - textSize.Height) / 2 - 50;
                    g.DrawString(gameOverText, gameOverFont, Brushes.Black, textX, textY);
                }

                // Nihai skoru çiz
                DrawScoreWithImages(g, gameCanvas.Height / 2 + 20);

                // Yeniden Başlama Talimatı
                Font restartFont = new Font("Arial", 16, FontStyle.Regular);
                string restartText = "Yeniden Başlamak İçin R Tuşuna Basın";
                SizeF restartSize = g.MeasureString(restartText, restartFont);
                float restartX = (gameCanvas.Width - restartSize.Width) / 2;
                float restartY = gameCanvas.Height / 2 + 60;
                g.DrawString(restartText, restartFont, Brushes.Black, restartX, restartY);
            }
        }

        private void DrawScoreWithImages(Graphics g, float yOffset = 10)
        {
            string scoreString = score.ToString();
            float totalWidth = 0;
            List<Image> scoreDigitImages = new List<Image>();

            foreach (char digitChar in scoreString)
            {
                int digit = int.Parse(digitChar.ToString());
                if (digit >= 0 && digit < 10 && scoreNumbers[digit] != null)
                {
                    scoreDigitImages.Add(scoreNumbers[digit]);
                    totalWidth += scoreNumbers[digit].Width;
                }
            }

            float currentX = (gameCanvas.Width - totalWidth) / 2;
            foreach (Image digitImage in scoreDigitImages)
            {
                g.DrawImage(digitImage, currentX, yOffset, digitImage.Width, digitImage.Height);
                currentX += digitImage.Width;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (currentState == GameState.Ready)
                {
                    currentState = GameState.Playing;
                    gameTimer.Start();
                    GenerateInitialPipes();
                }
                else if (currentState == GameState.Playing)
                {
                    birdSpeed = -10;
                }
            }

            if (e.KeyCode == Keys.R && currentState == GameState.GameOver)
            {
                RestartGame();
            }

            // Escape tuşuna basıldığında uygulamayı kapat
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void GenerateInitialPipes()
        {
            pipes.Clear();
            GenerateNewPipe(gameCanvas.Width + 100);
            GenerateNewPipe(gameCanvas.Width + 100 + distanceBetweenPipes);
            GenerateNewPipe(gameCanvas.Width + 100 + (2 * distanceBetweenPipes));
        }

        private void GenerateNewPipe(int xPosition = -1)
        {
            if (xPosition == -1)
                xPosition = pipes[pipes.Count - 1].X + distanceBetweenPipes;

            int minHeight = 50;
            int actualBaseHeight = (baseImage != null) ? baseImage.Height : 50;
            // Boru boşluğunun maksimum yüksekliğini hesaplarken pipeGap'i hesaba katıyoruz.
            // Oyun ilerledikçe pipeGap azalacağı için borular birbirine yaklaşacak.
            int maxHeight = gameCanvas.Height - pipeGap - minHeight - actualBaseHeight;
            if (maxHeight < minHeight) maxHeight = minHeight;

            int topPipeVisualHeight = rand.Next(minHeight, maxHeight);

            pipes.Add(new Rectangle(xPosition, -(pipeImage.Height - topPipeVisualHeight), pipeWidth, pipeImage.Height));
            int bottomPipeY = topPipeVisualHeight + pipeGap;
            pipes.Add(new Rectangle(xPosition, bottomPipeY, pipeWidth, pipeImage.Height));
        }

        private void CheckCollisions()
        {
            Rectangle birdBounds = new Rectangle(birdX, birdY, birdWidth, birdHeight);

            int actualBaseHeight = (baseImage != null) ? baseImage.Height : 50;
            if (birdY + birdHeight > gameCanvas.Height - actualBaseHeight || birdY < 0)
            {
                GameOver();
                return;
            }

            foreach (Rectangle pipe in pipes)
            {
                if (birdBounds.IntersectsWith(pipe))
                {
                    GameOver();
                    return;
                }
            }
        }

        private void UpdateScore()
        {
            if (currentState != GameState.Playing) return;

            for (int i = 0; i < pipes.Count; i += 2)
            {
                Rectangle topPipe = pipes[i];

                if (birdX > topPipe.Right && (birdX - pipeSpeed) <= topPipe.Right)
                {
                    score++;

                    // Her 20 skorda bir gece/gündüz modunu değiştir
                    if (score % 20 == 0)
                    {
                        ToggleDayNightMode();
                    }

                    // Her 10 skorda bir zorluğu artır (hız ve boru boşluğu)
                    if (score > 0 && score % difficultyIncreaseScoreInterval == 0)
                    {
                        IncreaseDifficulty();
                    }
                }
            }
        }

        private void ToggleDayNightMode()
        {
            if (currentBackgroundImage == backgroundDayImage)
            {
                currentBackgroundImage = backgroundNightImage;
            }
            else
            {
                currentBackgroundImage = backgroundDayImage;
            }
            gameCanvas.Invalidate();
        }

        private void IncreaseDifficulty() // YENİ METOT: Zorluğu artırır
        {
            // Boru hızını artır
            pipeSpeed++;
            // Boru boşluğunu azalt (minimum bir değere kadar)
            if (pipeGap > 120) // Örneğin, 120 pikselden daha küçük olmasın
            {
                pipeGap -= 10;
            }
            // Boru aralığını da biraz azaltabiliriz, daha sık boru gelmesi için
            // distanceBetweenPipes -= 10; // Çok hızlı zorlaşmaması için yorumda tuttum
        }


        private void GameOver()
        {
            currentState = GameState.GameOver;
            gameTimer.Stop();
        }

        private void RestartGame()
        {
            currentState = GameState.Ready;
            score = 0;
            birdY = 150;
            birdSpeed = 0;
            pipes.Clear();

            // Zorluk ayarlarını sıfırla
            pipeSpeed = initialPipeSpeed;
            pipeGap = initialPipeGap;

            currentBackgroundImage = backgroundDayImage;

            gameCanvas.Invalidate();
        }
    }
} 