using MyActivityCenter.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyActivityCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void ImgClose_MouseEnter(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(76, 110, 1000, 5);
            imgClose.BeginAnimation(Image.WidthProperty, db);
            imgClose.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgClose_MouseLeave(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(110, 76, 1000, 5);
            imgClose.BeginAnimation(Image.WidthProperty, db);
            imgClose.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(76, 110, 1000, 5);
            imgMinimize.BeginAnimation(Image.WidthProperty, db);
            imgMinimize.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(100, 76, 1000, 5);
            imgMinimize.BeginAnimation(Image.WidthProperty, db);
            imgMinimize.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(76, 100, 1000, 5);
            imgSettings.BeginAnimation(Image.WidthProperty, db);
            imgSettings.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(100, 76, 1000, 5);
            imgSettings.BeginAnimation(Image.WidthProperty, db);
            imgSettings.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgUpdateMyActivities_MouseEnter(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(76, 100, 1000, 5);
            imgUpdateMyActivities.BeginAnimation(Image.WidthProperty, db);
            imgUpdateMyActivities.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgUpdateMyActivities_MouseLeave(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(100, 76, 1000, 5);
            imgUpdateMyActivities.BeginAnimation(Image.WidthProperty, db);
            imgUpdateMyActivities.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // close program
            Application.Current.Shutdown();
        }

        private void ImgMinimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ImgUpdateMyActivities_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GoogleDriveService.GetInstance().ProcessDriveAsync();
        }

        private void ImgSettings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Opens Settings window.
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
            this.Hide();

        }

        private void Action_CategoryClick(object sender, MouseButtonEventArgs e)
        {

            var activitiesListWindow = new ActivitiesListWindow(); // You can add an indicator for the list to be shown
            switch (((Image)sender).Name)
            {
                case "imgPuzzles":
                    activitiesListWindow.Category = "Puzzles";
                    break;
                case "imgStoryTelling":
                    activitiesListWindow.Category = "Storytelling";
                    break;
                case "imgColouring":
                    activitiesListWindow.Category = "Colouring";
                    break;
                case "imgGames":
                    activitiesListWindow.Category = "Games";
                    break;
                case "imgCrafts":
                    activitiesListWindow.Category = "Crafts";
                    break;
                case "imgWorkSheets":
                    activitiesListWindow.Category = "Worksheets";
                    break;
                default:
                    break;
            }
            activitiesListWindow.Owner = this;
            activitiesListWindow.Show();
            this.Hide();
        }

        private void ImgAnimate_GotFocus(object sender, RoutedEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(100, 100 * 0.5, 1000, 5);
            (sender as Canvas).Background = Brushes.Black;
        }
    }
}
