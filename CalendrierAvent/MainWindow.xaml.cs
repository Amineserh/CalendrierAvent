using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;

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

        private bool _calendarShown;

        // Messages par jour (24 cases)
        private readonly string[] _giftMessages = new string[24]
        {
            "Jour 1 : Que cette période de Noël t'apporte douceur et chocolat chaud.",
            "Jour 2 : Prends un moment pour toi aujourd'hui, tu le mérites.",
            "Jour 3 : Un petit sourire peut illuminer toute une journée.",
            "Jour 4 : Souviens-toi : chaque effort compte, même les petits.",
            "Jour 5 : Aujourd'hui, fais quelque chose qui te rend vraiment heureux.",
            "Jour 6 : La magie de Noël, c'est aussi les gens qui t'entourent.",
            "Jour 7 : Tu avances, même quand tu as l'impression de stagner.",
            "Jour 8 : Une pause, un plaid, un film de Noël : programme validé.",
            "Jour 9 : Merci d'être toi, vraiment.",
            "Jour 10 : Tu as déjà surmonté tellement de choses. Continue.",
            "Jour 11 : Aujourd'hui, pense à un bon souvenir. Garde-le au chaud.",
            "Jour 12 : Tu comptes plus que tu ne le crois.",
            "Jour 13 : Un chocolat, un sourire, et ça repart.",
            "Jour 14 : Tu as le droit d'être fier de ton chemin.",
            "Jour 15 : Profite des petites choses : elles font les grands souvenirs.",
            "Jour 16 : Un message pour toi : tu n'es pas seul.",
            "Jour 17 : Tu peux être fatigué, mais tu restes capable.",
            "Jour 18 : Laisse un peu de place à la magie aujourd'hui.",
            "Jour 19 : Une journée de plus, une étape de plus.",
            "Jour 20 : Tu as déjà fait du bon travail cette année.",
            "Jour 21 : Bientôt Noël ! Accroche-toi encore un peu.",
            "Jour 22 : Prends le temps de respirer profondément.",
            "Jour 23 : Demain sera encore une occasion de briller.",
            "Jour 24 : Joyeux Noël ! Merci d'avoir ouvert ce calendrier avec moi."
        };

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
            TitlePanel.Opacity = 0;

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

        // ========= CLIQUE SUR LE SAPIN → CALENDRIER =========
        private void TreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_calendarShown)
                return;

            _calendarShown = true;
            ShowCalendar();
        }

        private void ShowCalendar()
        {
            CalendarOverlay.Visibility = Visibility.Visible;
            CalendarOverlay.Opacity = 0;

            var sb = (Storyboard)FindResource("CalendarAppearStoryboard");
            sb.Begin(this);

            // Verrouiller les cases en fonction du jour
            int today = DateTime.Now.Day;
            int maxDay = Math.Min(today, 24); // au-delà de 24, on s'en fiche

            int index = 0;
            foreach (UIElement child in CalendarGrid.Children)
            {
                if (child is Button btn)
                {
                    if (int.TryParse(btn.Content.ToString(), out int dayNum))
                    {
                        if (dayNum > maxDay)
                        {
                            btn.IsEnabled = false;
                            btn.Opacity = 0.35;
                        }
                        else
                        {
                            btn.IsEnabled = true;
                            btn.Opacity = 1.0;
                        }
                    }

                    // Animation d'arrivée des cases (vague + petit bounce)
                    var trans = new TranslateTransform();
                    btn.RenderTransform = trans;

                    double startY = -200;
                    var animY = new DoubleAnimation
                    {
                        From = startY,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new BounceEase
                        {
                            Bounces = 2,
                            Bounciness = 2,
                            EasingMode = EasingMode.EaseOut
                        },
                        BeginTime = TimeSpan.FromMilliseconds(40 * index)
                    };

                    double startX = (index % 2 == 0) ? -80 : 80;
                    var animX = new DoubleAnimation
                    {
                        From = startX,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                        BeginTime = TimeSpan.FromMilliseconds(40 * index)
                    };

                    trans.BeginAnimation(TranslateTransform.YProperty, animY);
                    trans.BeginAnimation(TranslateTransform.XProperty, animX);

                    index++;
                }
            }
        }

        // ========= CLICK SUR UNE CASE DU CALENDRIER =========
        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (!btn.IsEnabled)
                    return;

                if (!int.TryParse(btn.Content.ToString(), out int day))
                    return;

                ShowGiftForDay(day);
            }
        }

        private void ShowGiftForDay(int day)
        {
            // Titre
            GiftTitleText.Text = $"Cadeau du {day} décembre";

            // Message
            int idx = Math.Max(0, Math.Min(day - 1, _giftMessages.Length - 1));
            GiftMessageText.Text = _giftMessages[idx];

            // Image : Gifts/gift{day}.png
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = Path.Combine(baseDir, "Gifts", $"gift{day}.png");

            if (File.Exists(imgPath))
            {
                try
                {
                    GiftImage.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
                }
                catch
                {
                    GiftImage.Source = null;
                }
            }
            else
            {
                GiftImage.Source = null;
            }

            GiftPopup.Visibility = Visibility.Visible;
        }

        // ========= FERMER LA POPUP CADEAU =========
        private void CloseGiftPopupButton_Click(object sender, RoutedEventArgs e)
        {
            GiftPopup.Visibility = Visibility.Collapsed;
        }
    }
}