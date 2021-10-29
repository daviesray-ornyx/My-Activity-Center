using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActivityCenter.lib
{
    public class Settings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _sharedGoogleFolderPath;
        public string SharedGoogleFolderPath {
            get
            {
                return _sharedGoogleFolderPath;
            }
            set
            {
                if (value == _sharedGoogleFolderPath)
                    return;

                _sharedGoogleFolderPath = value;
                OnPropertyChanged("SharedGoogleFolderPath");
            }
        }

        private string _localResourcesFolder;
        public string LocalResourcesFolder
        {
            get
            {
                return _localResourcesFolder;
            }
            set
            {
                if (value == _localResourcesFolder)
                    return;
                _localResourcesFolder = value;
                OnPropertyChanged("LocalResourcesFolder");
            }
        }

        private string _onlineResourcesFilePath { get; set; } // Linked resources have the format [name, type(music/video), link]
        public string OnlineResourcesFilePath
        {
            get
            {
                return _onlineResourcesFilePath;
            }
            set
            {
                if (value == _onlineResourcesFilePath)
                    return;
                _onlineResourcesFilePath = value;
                OnPropertyChanged("OnlineResourcesFilePath");
            }
        }

        public DateTime _lastUpdateDate { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) // if there is any subscribers 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "LocalResourcesFolder")
            {
                OnlineResourcesFilePath = _localResourcesFolder + @"\Online Resources.xlsx";
            }
        }
    }
}
