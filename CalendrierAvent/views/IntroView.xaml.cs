using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CalendrierAvent.Views
{
    public partial class IntroView : UserControl
    {
        public event Action IntroCompleted;

        private MediaPlayer _musicPlayer;

        public IntroView()
        {
            InitializeComponent();
            Loaded += IntroView_Loaded;
        }

        private void IntroView_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Vidéo playbackvideo.mp4
            string videoPath = Path.Combine(baseDir, "Assets", "playbackvideo.mp4");
            if (File.Exists(videoPath))
            {
                IntroVideo.Source = new Uri(videoPath, UriKind.Absolute);
                IntroVideo.Play();
            }
            else
            {
                MessageBox.Show("Vidéo introuvable : " + videoPath);
                OnIntroCompleted();
                return;
            }

            // Musique Alliwant.mp3
            try
            {
                string musicPath = Path.Combine(baseDir, "Son", "Alliwant.mp3");
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
                MessageBox.Show("Erreur musique : " + ex.Message);
            }
        }

        private void StopEverything()
        {
            try { IntroVideo.Stop(); } catch { }
            try { if (_musicPlayer != null) _musicPlayer.Stop(); } catch { }
        }

        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopEverything();
            OnIntroCompleted();
        }

        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("La vidéo ne peut pas être lue.\n\nErreur : " + e.ErrorException.Message);
            StopEverything();
            OnIntroCompleted();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            NamePopup.Visibility = Visibility.Visible;
            NameTextBox.Text = "";
            NameTextBox.Focus();
        }

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
            StopEverything();
            OnIntroCompleted();
        }

        private void OnIntroCompleted()
        {
            if (IntroCompleted != null)
                IntroCompleted();
        }
    }
}
