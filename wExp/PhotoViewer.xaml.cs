using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace wExp
{
    /// <summary>
    /// Interaction logic for PhotoViewer.xaml
    /// </summary>
    public partial class PhotoViewer : Window
    {
        private string PathViewer { get; set; }
        private int CurrentViewer { get; set; }
        private string[] ListViewer { get; set; }

        public PhotoViewer(string currentImage)
        {
            InitializeComponent();
            PathViewer = System.IO.Path.GetDirectoryName(currentImage);

            ListViewer = Directory.GetFiles(PathViewer, "*.jpg", SearchOption.AllDirectories);

            for (int i = 0; i < ListViewer.Length; i++)
                if (ListViewer[i] == currentImage)
                {
                    CurrentViewer = i;
                    break;
                }

            Viewer(ListViewer[CurrentViewer]);
        }

        public void Viewer(string imagePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imagePath);
            image.EndInit();

            imageView.Source = image;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewer++;
            if (CurrentViewer > (ListViewer.Length - 1))
                CurrentViewer = 0;
            Viewer(ListViewer[CurrentViewer]);
        }

        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewer--;
            if (CurrentViewer < 0)
                CurrentViewer = ListViewer.Length - 1;
            Viewer(ListViewer[CurrentViewer]);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                buttonBack_Click(sender, new RoutedEventArgs());

            if (e.Key == Key.Right)
                buttonForward_Click(sender, new RoutedEventArgs());

            //if (e.Key == Key.Up)
            //    imageView.Height += 10;

            //if (e.Key == Key.Down)
            //    imageView.Height -= 10;
        }
    }
}
