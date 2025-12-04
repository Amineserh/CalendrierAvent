using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CalendrierAvent.Views
{
    public partial class DoorView : UserControl
    {
        public event Action DoorOpened;

        public DoorView()
        {
            InitializeComponent();
            Loaded += DoorView_Loaded;
        }

        private void DoorView_Loaded(object sender, RoutedEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = Path.Combine(baseDir, "image", "portenoel.jpg");

            if (File.Exists(imgPath))
            {
                DoorImage.Source = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show("Image introuvable : " + imgPath);
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoorOpened != null)
                DoorOpened();
        }
    }
}
