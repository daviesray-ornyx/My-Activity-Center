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

        private bool listBusyIndicatorOn = false;
        private bool previewBusyIndicatorOn = false;

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

        private List<ResourceFile> _filesInCategory;
        public List<ResourceFile> FilesInCategory
        {
            get
            {
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

        private ResourceFile _selectedFile;
        public ResourceFile SelectedFile
        {
            get
            {
                if (_selectedFile == null)
                {
                    // create a default selected file
                    _selectedFile = new ResourceFile()
                    {
                        NameWithExtension = "doc-paceholder.png",
                        Name = "Placeholder",
                        FullPath = "images/doc-placeholder.png",
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

            gridPreview.DataContext = SelectedFile;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<ResourceFile> GetFilesInCategory()
        {
            var currentCategoryPath = baseDirectory;
            if (Category != ResourceCategories.All.ToString())
            {
                currentCategoryPath = System.IO.Path.Combine(baseDirectory, Category);
            }


            // We've got the full path. Next get all files in the directory
            var LocalFiles = new List<ResourceFile>();

            if (!Directory.Exists(currentCategoryPath))
            {
                Directory.CreateDirectory(currentCategoryPath);
                return LocalFiles;
            }

            var filePaths = Directory.GetFiles(currentCategoryPath, "*", SearchOption.AllDirectories).Where(item => !item.Contains("preview-cache") && !item.Contains("~$")); //Exclude cache folders
            
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                var file = new ResourceFile()
                {
                    NameWithExtension = fileInfo.Name,
                    Name = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    FullPath = filePath,
                };

                //file.CreatePreviewFile(); // Preview file only created on demand
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
            SelectedFile = (ResourceFile)lvActivities.SelectedItem;

            imgPreview.Source = new BitmapImage(new Uri(SelectedFile.PreviewPath)); ; //TODO: Use a try and replace with doc-placeholder if non existent
        }

        private void Action_OpenResource(object sender, RoutedEventArgs e)
        {
            if (SelectedFile.Name == "Placeholder")
                return;
            Process.Start(SelectedFile.FullPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleListBusyIndicator(true);
            if (FilesInCategory == null)
            {
                // Begin getting them Files
                FilesInCategory = GetFilesInCategory();
                lvActivities.ItemsSource = FilesInCategory;
            }

            // Set selected File
            lvActivities.SelectedIndex = 0;
            
            ToggleListBusyIndicator(false);
        }

        private void ToggleListBusyIndicator(bool show)
        {
            if (show)
            {
                listBusyIndicatorOn = true;
                ListBusyIndicatorRow.Height = new GridLength(100);
            }
            else
            {
                listBusyIndicatorOn = false;
                ListBusyIndicatorRow.Height = new GridLength(0);
            }
        }

        private void TogglePreviewBusyIndicator()
        {

        }
    }
}
