using System;
using System.Windows;
using System.Windows.Media.Animation;
using CalendrierAvent.Views;

namespace CalendrierAvent
{
    public partial class MainWindow : Window
    {
        private bool _titleShown;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Calendrier de l'Avent";
            DateText.Text = DateTime.Now.ToString("dd MMMM yyyy");

            ShowIntroView();
        }

        private void ShowIntroView()
        {
            IntroView intro = new IntroView();
            intro.IntroCompleted += Intro_IntroCompleted;
            MainContent.Content = intro;
        }

        private void Intro_IntroCompleted()
        {
            ShowTitlePanel();
            ShowDoorView();
        }

        private void ShowDoorView()
        {
            DoorView door = new DoorView();
            door.DoorOpened += Door_DoorOpened;
            MainContent.Content = door;
        }

        private void Door_DoorOpened()
        {
            ShowCalendarView();
        }

        private void ShowCalendarView()
        {
            CalendarView cal = new CalendarView();
            MainContent.Content = cal;
        }

        private void ShowTitlePanel()
        {
            if (_titleShown)
                return;

            _titleShown = true;
            Storyboard sb = (Storyboard)FindResource("TitleAppearStoryboard");
            sb.Begin(this);
        }
    }
}
