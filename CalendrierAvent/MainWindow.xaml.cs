using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace CalendrierAvent
{
    public partial class MainWindow : Window
    {
        private MediaPlayer _musicPlayer;
        private MediaPlayer _secondMusicPlayer;
        private DispatcherTimer _delayTimer;
        private bool _titleShown;
        private Storyboard _titleStoryboard;
        private string _fullTitle;
        private string _fullDate;
        public MainWindow()
        {
            InitializeComponent();
        }
        // ========= CHARGEMENT FENÊTRE =========
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // ----- VIDÉO 1 -----
            string videoPath = Path.Combine(baseDir, "Assets", "playbackvideo.mp4");
            if (!File.Exists(videoPath))
            {
                MessageBox.Show("Vidéo introuvable : " + videoPath);
                return;
            }
            IntroVideo.Source = new Uri(videoPath, UriKind.Absolute);
            IntroVideo.Play();
            // ----- MUSIQUE 1 -----
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
            // ----- TEXTE (sans machine à écrire) -----
            _fullTitle = "Calendrier de l'Avent";
            _fullDate = DateTime.Now.ToString("dd MMMM yyyy");
            TitleText.Text = _fullTitle;
            DateText.Text = _fullDate;
            TitlePanel.Opacity = 0; // on laisse l'anim faire le fondu
            _titleStoryboard = (Storyboard)FindResource("TitleAppearStoryboard");
            // Lancer automatiquement le titre après 20 s si personne ne skip
            _delayTimer = new DispatcherTimer();
            _delayTimer.Interval = TimeSpan.FromSeconds(20);
            _delayTimer.Tick += DelayTimer_Tick;
            _delayTimer.Start();
        }
        // ========= AFFICHER LE TITRE (anim fluide) =========
        private void ShowTitlePanel()
        {
            if (_titleShown)
                return;
            _titleShown = true;
            if (_delayTimer != null)
                _delayTimer.Stop();
            _titleStoryboard.Begin(this);
        }
        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            ShowTitlePanel();
        }
        // ========= BOUTON SKIP → popup prénom =========
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            NamePopup.Visibility = Visibility.Visible;
            NameTextBox.Text = "";
            NameTextBox.Focus();
        }
        // ========= VALIDATION DU PRÉNOM =========
        private void ConfirmNameButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Entre ton prénom pour continuer 😄", "Ho ho ho !");
                return;
            }
            MessageBox.Show(
                "Ahhh, c'est toi " + name + " !\nAllez viens, rentre récupérer ton cadeau de la journée ! 🎁",
                "Père Noël");
            NamePopup.Visibility = Visibility.Collapsed;
            GoToDoor();
        }
        // ========= PASSER À L'ÉCRAN DE LA PORTE =========
        private void GoToDoor()
        {
            try
            {
                IntroVideo.Stop();
            }
            catch
            {
            }
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = Path.Combine(baseDir, "image", "portenoel.jpg");
            IntroGrid.Visibility = Visibility.Collapsed;
            DoorGrid.Visibility = Visibility.Visible;
            NamePopup.Visibility = Visibility.Collapsed;
            if (File.Exists(imgPath))
            {
                DoorImage.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Image introuvable : " + imgPath);
            }
            // On lance une fois l'anim propre du titre + date
            ShowTitlePanel();
        }
        // ========= FIN DE LA VIDÉO =========
        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            GoToDoor();
        }
        // ========= VIDÉO QUI BUG =========
        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("La vidéo ne peut pas être lue.\n\nErreur : " + e.ErrorException.Message);
            GoToDoor();
        }
        // ========= BOUTON ENTRER → 2e PAGE =========
        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            DoorGrid.Visibility = Visibility.Collapsed;
            NextPageGrid.Visibility = Visibility.Visible;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // 🎥 Nouvelle vidéo : interieur.mp4
            string secondVideoPath = Path.Combine(baseDir, "Assets", "interieur.mp4");
            if (File.Exists(secondVideoPath))
            {
                NextVideo.Source = new Uri(secondVideoPath, UriKind.Absolute);
                NextVideo.Play();
                NextPageText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NextPageText.Text = "Vidéo introuvable : " + secondVideoPath;
                NextPageText.Visibility = Visibility.Visible;
            }
            // 🎵 Nouvelle musique : fortnite.mp3
            try
            {
                if (_musicPlayer != null)
                    _musicPlayer.Stop();
                string secondMusicPath = Path.Combine(baseDir, "Son", "fortnite.mp3");
                if (File.Exists(secondMusicPath))
                {
                    _secondMusicPlayer = new MediaPlayer();
                    _secondMusicPlayer.Open(new Uri(secondMusicPath, UriKind.Absolute));
                    _secondMusicPlayer.Volume = 0.6;
                    _secondMusicPlayer.MediaEnded += (s, ev) =>
                    {
                        _secondMusicPlayer.Position = TimeSpan.Zero;
                        _secondMusicPlayer.Play();
                    };
                    _secondMusicPlayer.Play();
                }
                else
                {
                    MessageBox.Show("Musique introuvable : " + secondMusicPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur musique 2 : " + ex.Message);
            }
        }
    }
}