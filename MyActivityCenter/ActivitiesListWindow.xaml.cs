using MyActivityCenter.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyActivityCenter
{
    /// <summary>
    /// Interaction logic for ActivitiesListWindow.xaml
    /// </summary>
    public partial class ActivitiesListWindow : Window
    {
        private string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\My Activity Center";

             

        public event PropertyChangedEventHandler PropertyChanged;
        private enum ResourceCategories
        {
            All, Crafts, Colouring, Games, Puzzles, Storytelling, Worksheets
        }

        private string _category;
        public string Category {
            get
            {
                if (_category == null)
                    _category = ResourceCategories.All.ToString();                
                return _category;
            }
            set
            {
                if (value == _category)
                    return;
                _category = value;
                OnPropertyChanged("Category");
            }
        }

        private List<LocalFile> _filesInCategory;
        public List<LocalFile> FilesInCategory
        {
            get
            {
                if (_filesInCategory == null)
                    _filesInCategory = GetFilesInCategory();
                return _filesInCategory;
            }
            set
            {
                if (value == _filesInCategory)
                    return;
                _filesInCategory = value;
                OnPropertyChanged("FilesInCategory");
            }

        }

        public ActivitiesListWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<LocalFile> GetFilesInCategory()
        {
            var currentCategoryPath = baseDirectory;
            if (Category != ResourceCategories.All.ToString())
            {
                currentCategoryPath = System.IO.Path.Combine(baseDirectory, Category);
            }

            // We've got the full path. Next get all files in the directory
            var filePaths = Directory.GetFiles(currentCategoryPath, "*", SearchOption.AllDirectories);
            var LocalFiles = new List<LocalFile>();

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                LocalFiles.Add(
                    new LocalFile()
                    {
                        NameWithExtension = fileInfo.Name,
                        Name = System.IO.Path.GetFileNameWithoutExtension(filePath),
                        FullPath = filePath,
                        DirectoryPath = fileInfo.DirectoryName
                    }
                );
            }
            return LocalFiles;
        }


        /*
         *
         * Event handlers section
         *
         **/

        private void Action_BackHome(object sender, MouseButtonEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void ImgBack_MouseEnter(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(76, 100, 1000, 5);
            ImgBack.BeginAnimation(Image.WidthProperty, db);
            ImgBack.BeginAnimation(Image.HeightProperty, db);
        }

        private void ImgBack_MouseLeave(object sender, MouseEventArgs e)
        {
            var db = Animate.GetInstance().GetMenuDoubleAnimation(100, 76, 1000, 5);
            ImgBack.BeginAnimation(Image.WidthProperty, db);
            ImgBack.BeginAnimation(Image.HeightProperty, db);
        }

        private void ListViewItem_PreviewMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            var localFile = (LocalFile)((ListBoxItem)sender).DataContext;
            System.Diagnostics.Process.Start(localFile.FullPath);
        }

        private void ListViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            // lvActivities.SelectedItem
            // Leaving selected item            
        }

    }
}
