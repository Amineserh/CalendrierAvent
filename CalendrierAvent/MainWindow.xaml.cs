using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace CalendrierAvent
{
    public partial class MainWindow : Window
    {
        private MediaPlayer _musicPlayer;
        private DispatcherTimer _delayTimer;
        private DispatcherTimer _typingTimer;
        private string _fullTitle;
        private string _fullDate;
        private bool _typingTitle;
        private int _charIndex;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // ====== VIDÉO ======
            string videoPath = Path.Combine(baseDir, "Assets", "playbackvideo.mp4");
            if (!File.Exists(videoPath))
            {
                MessageBox.Show("Vidéo introuvable : " + videoPath);
                return;
            }
            IntroVideo.Source = new Uri(videoPath, UriKind.Absolute);
            IntroVideo.Play();
            // ====== MUSIQUE ======
            try
            {
                string musicPath = Path.Combine(baseDir, "Son", "Alliwant.mp3");
                if (File.Exists(musicPath))
                {
                    _musicPlayer = new MediaPlayer();
                    _musicPlayer.Open(new Uri(musicPath, UriKind.Absolute));
                    _musicPlayer.Volume = 0.6;
                    _musicPlayer.MediaEnded += (s, ev) =>
                    {
                        _musicPlayer.Position = TimeSpan.Zero;
                        _musicPlayer.Play();
                    };
                    _musicPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur musique : " + ex.Message);
            }
            // ====== TEXTE ======
            _fullTitle = "Calendrier de l'Avent";
            _fullDate = DateTime.Now.ToString("dd MMMM yyyy");
            TitlePanel.Opacity = 0;
            TitleText.Text = "";
            DateText.Text = "";
            // AFFICHAGE TITRE + DATE APRÈS 20s
            _delayTimer = new DispatcherTimer();
            _delayTimer.Interval = TimeSpan.FromSeconds(20);
            _delayTimer.Tick += DelayTimer_Tick;
            _delayTimer.Start();
        }
        // FIN DE LA VIDÉO → AFFICHE LA PORTE
        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = Path.Combine(baseDir, "image", "portenoel.jpg");
            IntroGrid.Visibility = Visibility.Collapsed;
            DoorGrid.Visibility = Visibility.Visible;
            if (File.Exists(imgPath))
            {
                DoorImage.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Image introuvable : " + imgPath);
            }
        }
        // ANIMATION TEXTE
        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            _delayTimer.Stop();
            TitlePanel.Opacity = 1;
            _typingTitle = true;
            _charIndex = 0;
            _typingTimer = new DispatcherTimer();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(100);
            _typingTimer.Tick += TypingTimer_Tick;
            _typingTimer.Start();
        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (_typingTitle)
            {
                if (_charIndex < _fullTitle.Length)
                {
                    TitleText.Text += _fullTitle[_charIndex];
                    _charIndex++;
                }
                else
                {
                    _typingTitle = false;
                    _charIndex = 0;
                }
            }
            else
            {
                if (_charIndex < _fullDate.Length)
                {
                    DateText.Text += _fullDate[_charIndex];
                    _charIndex++;
                }
                else
                {
                    _typingTimer.Stop();
                }
            }
        }
        // BOUTON ENTRER → PAGE BLANCHE
        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            DoorGrid.Visibility = Visibility.Collapsed;
            NextPageGrid.Visibility = Visibility.Visible;
        }
        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("La vidéo ne peut pas être lue.\n\nErreur : " + e.ErrorException.Message);
        }
    }
}