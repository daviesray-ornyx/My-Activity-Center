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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyActivityCenter
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// 
    public partial class SettingsWindow : Window
    {
        private Settings _currentSettings;
        public Settings CurrentSettings
        {
            get
            {
                if (_currentSettings == null)
                    _currentSettings = SettingsManager.GetInstance().GetSettings();
                return _currentSettings;
            }
            set
            {
                if (value == _currentSettings)
                    return;
                _currentSettings = value;
                // Don't handle change yet
            }
        }
        public Settings settings = SettingsManager.GetInstance().GetSettings();

        public SettingsWindow()
        {
            InitializeComponent();

            this.DataContext = CurrentSettings;
            // Default center screen
            // Works with a singleton that keeps track of the setings. We could have these stored in a json file

            // Links are also saved as well in the same json file and object
        }

        public void SelectFolderBrowser(string pathName, bool showNewFolderButton, string selectedPath)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowNewFolderButton = showNewFolderButton;
            dialog.SelectedPath = selectedPath; // CurrentSettings.LocalResourcesFolder;

            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                switch (pathName)
                {
                    case "LocalResourcesFolder":
                        CurrentSettings.LocalResourcesFolder = dialog.SelectedPath;
                        break;
                    default:
                        break;
                }
                
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Save Settings
            if (SettingsManager.GetInstance().SaveSettingsAsync(settings))
                this.Close();
            
        }
        
        private void BtnBrowseResourceFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            SelectFolderBrowser("LocalResourcesFolder", true, CurrentSettings.LocalResourcesFolder);
        }
    }
}
