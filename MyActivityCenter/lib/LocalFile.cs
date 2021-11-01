using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActivityCenter.lib
{
    public class LocalFile
    {
        public string Name { get; set; }
        public string NameWithExtension { get; set; }
        public string DirectoryPath { get; set; }
        public string FullPath { get; set; }
        private string _previewPath;
        public string PreviewPath {
            get
            {
                if (String.IsNullOrEmpty(_previewPath))
                {
                    // try getting the path again
                    Directory.CreateDirectory(Path.Combine(DirectoryPath, @"preview-cache")); // create directory if it does not exist

                    // Try setting the _previewPath
                    _previewPath =
                }

                _previewPath = Path.Combine(DirectoryPath, @"preview-cache", NameWithExtension);
                return _previewPath;
            }
            set
            {
                if (value == _previewPath)
                    return;
                _previewPath = value;
            }
        }
    }
}
