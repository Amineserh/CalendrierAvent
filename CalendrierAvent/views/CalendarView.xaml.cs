using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CalendrierAvent.Views
{
    public partial class CalendarView : UserControl
    {
        private MediaPlayer _musicPlayer;

        public CalendarView()
        {
            InitializeComponent();
            Loaded += CalendarView_Loaded;
        }

        private void CalendarView_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Vidéo interieur.mp4
            string videoPath = Path.Combine(baseDir, "Assets", "interieur.mp4");
            if (File.Exists(videoPath))
            {
                NextVideo.Source = new Uri(videoPath, UriKind.Absolute);
                NextVideo.Play();
                NextPageText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NextPageText.Text = "Vidéo introuvable : " + videoPath;
            }

            // Musique fortnite.mp3
            try
            {
                string musicPath = Path.Combine(baseDir, "Son", "fortnite.mp3");
                if (File.Exists(musicPath))
                {
                    _musicPlayer = new MediaPlayer();
                    _musicPlayer.Open(new Uri(musicPath, UriKind.Absolute));
                    _musicPlayer.Volume = 0.6;
                    _musicPlayer.MediaEnded += delegate
                    {
                        _musicPlayer.Position = TimeSpan.Zero;
                        _musicPlayer.Play();
                    };
                    _musicPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur musique 2 : " + ex.Message);
            }

            InitCalendarButtons();
        }

        private void InitCalendarButtons()
        {
            DateTime now = DateTime.Now;

            foreach (object child in CalendarGrid.Children)
            {
                Button btn = child as Button;
                if (btn == null)
                    continue;

                int day;
                if (!int.TryParse(btn.Content.ToString(), out day))
                    continue;

                if (now.Month == 12 && day <= now.Day)
                    btn.IsEnabled = true;
                else
                    btn.IsEnabled = false;
            }
        }

        private void TreeButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarOverlay.Visibility = Visibility.Visible;
            Storyboard sb = (Storyboard)FindResource("CalendarAppearStoryboard");
            sb.Begin(this);
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                return;

            int day;
            if (!int.TryParse(btn.Content.ToString(), out day))
                return;

            ShowGiftForDay(day);

            btn.IsEnabled = false; // marquer comme ouvert
        }

        private void ShowGiftForDay(int day)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string giftsDir = Path.Combine(baseDir, "Assets", "Gifts");

            // Image : gift{day}.png/jpg
            string imagePath = null;
            string png = Path.Combine(giftsDir, "gift" + day + ".png");
            string jpg = Path.Combine(giftsDir, "gift" + day + ".jpg");

            if (File.Exists(png)) imagePath = png;
            else if (File.Exists(jpg)) imagePath = jpg;

            if (imagePath != null)
            {
                GiftImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                GiftImage.Visibility = Visibility.Visible;
            }
            else
            {
                GiftImage.Source = null;
                GiftImage.Visibility = Visibility.Collapsed;
            }

            // Message : message{day}.txt optionnel
            string messagePath = Path.Combine(giftsDir, "message" + day + ".txt");
            string message;
            if (File.Exists(messagePath))
                message = File.ReadAllText(messagePath);
            else
                message = "Joyeux calendrier ! Voici le cadeau du " + day + " décembre 🎁";

            GiftTitleText.Text = "Jour " + day;
            GiftMessageText.Text = message;

            GiftPopup.Visibility = Visibility.Visible;
        }

        private void CloseGiftPopupButton_Click(object sender, RoutedEventArgs e)
        {
            GiftPopup.Visibility = Visibility.Collapsed;
        }
    }
}
