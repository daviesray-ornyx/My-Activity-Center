using MyActivityCenter.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace MyActivityCenter.lib
{
    class SettingsManager
    {
        private SettingsManager() { }
        private static SettingsManager _instance;
        private Settings settings;

        private string _baseDirectory;
        public string BaseDirectory
        {
            get
            {
                if (_baseDirectory == null)
                    _baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\My Activity Center";
                if (!Directory.Exists(_baseDirectory))
                    Directory.CreateDirectory(_baseDirectory);
                return _baseDirectory;
            }
            set
            {
                if (value == _baseDirectory)
                    return;
                _baseDirectory = value;
            }
        }

        private string settingsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\My Activity Center\" + @Resources.settings_file_name;

        public static SettingsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsManager();
            }
            return _instance;
        }

        public bool SaveSettingsAsync(Settings setting)
        {
            try
            {
                string settingsJSON = JsonConvert.SerializeObject(settings, Formatting.Indented);

                // Create directories that don't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
                FileStream fileW = new FileStream(settingsFilePath, FileMode.Create, FileAccess.Write);
                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(fileW);
                sw.Write(settingsJSON);
                sw.Flush();
                sw.Close();
                fileW.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error saving settings", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;            
        }

        public Settings GetSettings()
        {
            // Get some business logic here
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string settingsString = File.ReadAllText(settingsFilePath);
                    settings = JsonConvert.DeserializeObject<Settings>(settingsString);
                }
                else
                {
                    var OnlineResourcesFilePath = BaseDirectory + @"\Online Resources.xlsx";
                    settings = new Settings()
                    {
                        LocalResourcesFolder = BaseDirectory,
                        OnlineResourcesFilePath = OnlineResourcesFilePath
                    };

                    SaveSettingsAsync(settings);
                    //File.SetAttributes(settingsFilePath, File.GetAttributes(settingsFilePath) | FileAttributes.Hidden);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return null;
            }

            return settings; ;
        }

    }
}
