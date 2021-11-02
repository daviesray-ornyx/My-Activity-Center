using MyActivityCenter.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private LocalFile _selectedFile;
        public LocalFile SelectedFile
        {
            get
            {
                if (_selectedFile == null)
                {
                    // create a default selected file
                    _selectedFile = new LocalFile()
                    {
                        NameWithExtension = "doc-paceholder.png",
                        Name = "Placeholder",
                        FullPath = "images/doc-placeholder.png",
                        DirectoryPath = "images",
                        PreviewPath = "/MyActivityCenter;component/images/doc-placeholder.png"

                    };
                };
                return _selectedFile;
            }
            set
            {
                if (_selectedFile == value)
                    return;
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public ActivitiesListWindow()
        {
            InitializeComponent();
            DataContext = this;
            gridPreview.DataContext = SelectedFile;
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
            var LocalFiles = new List<LocalFile>();

            if (!Directory.Exists(currentCategoryPath))
            {
                Directory.CreateDirectory(currentCategoryPath);
                return LocalFiles;
            }

            var filePaths = Directory.GetFiles(currentCategoryPath, "*", SearchOption.AllDirectories).Where(item => !item.Contains("preview-cache")); //Exclude cache folders
            
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                var file = new LocalFile()
                {
                    NameWithExtension = fileInfo.Name,
                    Name = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    FullPath = filePath,
                    DirectoryPath = fileInfo.DirectoryName
                };

                file.CreatePreviewFile(); // Create preview file on creation
                LocalFiles.Add(file);
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
        
        private void LvActivities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvActivities.SelectedItem == null)
                return;
            SelectedFile = (LocalFile)lvActivities.SelectedItem;

            //TODO: Ensure the right type's are always there
            // 
            imgPreview.Source = new BitmapImage(new Uri(SelectedFile.PreviewPath)); ; //TODO: Use a try and replace with doc-placeholder if non existent
        }

        private void Action_OpenResource(object sender, RoutedEventArgs e)
        {
            if (SelectedFile.Name == "Placeholder")
                return;
            Process.Start(SelectedFile.FullPath);
        }
    }
}
