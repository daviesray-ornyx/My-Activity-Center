using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
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
        private GhostscriptVersionInfo _version = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);

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
                    _previewPath = Path.Combine(DirectoryPath, @"preview-cache", Name + ".jpeg");
                }
                
                
                return _previewPath;
            }
            set
            {
                if (value == _previewPath)
                    return;
                _previewPath = value;
            }
        }

        private bool CreatePDFPreviewFile()
        {
            try
            {
                using (var ghostscriptRasterizer = new GhostscriptRasterizer())
                {
                    ghostscriptRasterizer.Open(FullPath, _version, true);
                    var img = ghostscriptRasterizer.GetPage(96, 1);
                    img.Save(PreviewPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }                
            }
            catch (Exception ex)
            {
                // Failed to create preview. Return false
                return false;
            }

            return true;

        }

        private bool CreateDocPreviewFile()
        {

            return true;
        }

        public void CreatePreviewFile()
        {
            // check if previewPath exists
            if (!File.Exists(PreviewPath))
            {
                switch (Path.GetExtension(NameWithExtension))
                {
                    case ".pdf":
                        // Create preview for a pdf
                        CreatePDFPreviewFile();
                        break;
                    case ".doc":
                        // Create preview for .doc
                        break;
                    case ".docx":
                        // create preview for .docx
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
