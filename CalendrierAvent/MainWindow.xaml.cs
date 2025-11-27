using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
namespace CalendrierAvent
{
    public partial class MainWindow : Window
    {
        private MediaPlayer _musicPlayer;
        private DispatcherTimer _delayTimer;   // timer pour attendre 20s
        private DispatcherTimer _typingTimer;  // timer pour écrire lettre par lettre
        private string _fullTitle;
        private string _fullDate;
        private bool _typingTitle;             // true = on écrit le titre, false = on écrit la date
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
            // ====== MUSIQUE DE FOND ======
            try
            {
                string musicPath = Path.Combine(baseDir, "Son", "Alliwant.mp3");
                if (File.Exists(musicPath))
                {
                    _musicPlayer = new MediaPlayer();
                    _musicPlayer.Open(new Uri(musicPath, UriKind.Absolute));
                    _musicPlayer.Volume = 0.6;
                    _musicPlayer.MediaEnded += MusicPlayer_MediaEnded;
                    _musicPlayer.Play();
                }
                else
                {
                    MessageBox.Show("Musique introuvable : " + musicPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur musique : " + ex.Message);
            }
            // ====== TEXTE & ANIMATION ======
            _fullTitle = "Calendrier de l'Avent";
            _fullDate = DateTime.Now.ToString("dd MMMM yyyy");
            TitlePanel.Opacity = 0;
            TitleText.Text = "";
            DateText.Text = "";
            // On attend 20 secondes avant de commencer à afficher le texte
            _delayTimer = new DispatcherTimer();
            _delayTimer.Interval = TimeSpan.FromSeconds(20);
            _delayTimer.Tick += DelayTimer_Tick;
            _delayTimer.Start();
        }
        // Boucle la musique
        private void MusicPlayer_MediaEnded(object sender, EventArgs e)
        {
            _musicPlayer.Position = TimeSpan.Zero;
            _musicPlayer.Play();
        }
        // Boucle la vidéo (même si elle finit en noir, elle repart)
        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            IntroVideo.Position = TimeSpan.Zero;
            IntroVideo.Play();
        }
        // Après 20s, on lance l'effet "machine à écrire"
        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            _delayTimer.Stop();
            TitlePanel.Opacity = 1;  // on rend le panel visible
            _typingTitle = true;     // on commence par écrire le titre
            _charIndex = 0;
            _typingTimer = new DispatcherTimer();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(100); // vitesse d'écriture
            _typingTimer.Tick += TypingTimer_Tick;
            _typingTimer.Start();
        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (_typingTitle)
            {
                // On écrit le titre lettre par lettre
                if (_charIndex < _fullTitle.Length)
                {
                    TitleText.Text += _fullTitle[_charIndex];
                    _charIndex++;
                }
                else
                {
                    // Titre fini -> on passe à la date
                    _typingTitle = false;
                    _charIndex = 0;
                }
            }
            else
            {
                // On écrit la date lettre par lettre
                if (_charIndex < _fullDate.Length)
                {
                    DateText.Text += _fullDate[_charIndex];
                    _charIndex++;
                }
                else
                {
                    // Animation terminée
                    _typingTimer.Stop();
                }
            }
        }
        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("La vidéo ne peut pas être lue.\n\nErreur : " + e.ErrorException.Message);
        }
    }
}