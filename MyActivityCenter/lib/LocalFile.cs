using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Core;

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

        private Microsoft.Office.Interop.Word.Application _msWordApp;
        public Microsoft.Office.Interop.Word.Application MSWordApp
        {
            get
            {
                if (_msWordApp == null)
                {
                    try
                    {
                        _msWordApp = new Microsoft.Office.Interop.Word.Application();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }                    
                return _msWordApp;
            }
            set
            {
                if (_msWordApp == value)
                    return;
                _msWordApp = value;
            }
        }

        private Microsoft.Office.Interop.PowerPoint.Application _msPPTApp;
        public Microsoft.Office.Interop.PowerPoint.Application MSPPTApp
        {
            get
            {
                if (_msPPTApp == null)
                {
                    try
                    {
                        _msPPTApp = new Microsoft.Office.Interop.PowerPoint.Application();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                return _msPPTApp;
            }
            set
            {
                if (_msPPTApp == value)
                    return;
                _msPPTApp = value;
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
                CreatePlaceholderPreviewFile();
            }

            return true;

        }

        private bool CreateDocxPreviewFile()
        {
            
            try
            {
                var doc = MSWordApp.Documents.Open(FullPath, Visible: false);
                doc.ShowGrammaticalErrors = false;
                doc.ShowRevisions = false;
                doc.ShowSpellingErrors = false;
                if (doc.Windows.Count > 0 && doc.Windows[0].Panes.Count > 0)
                {
                    var bits = doc.Windows[0].Panes[0].Pages[0].EnhMetaFileBits;
                    using (var memoryStream = new MemoryStream((byte[])(bits)))
                    {
                        var image = System.Drawing.Image.FromStream(memoryStream);
                        image.Save(PreviewPath, ImageFormat.Jpeg);
                    }
                }

                doc.Close(Type.Missing, Type.Missing, Type.Missing);
                
            }
            catch (System.Exception ex)
            {
                // No preview generated
                // Copy the default image to preview path
                CreatePlaceholderPreviewFile();
            }

            return true;
        }
                
        private bool CreatePPTPreviewFile()
        {
            try
            {
                var pptPresentation = MSPPTApp.Presentations.Open(FullPath, MsoTriState.msoFalse, MsoTriState.msoFalse, WithWindow: MsoTriState.msoFalse);

                pptPresentation.Slides[1].Export(PreviewPath, "jpg", Int32.Parse(pptPresentation.SlideMaster.Width.ToString()), Int32.Parse(pptPresentation.SlideMaster.Height.ToString()));

                pptPresentation.Close();
            }
            catch (System.Exception ex)
            {
                CreatePlaceholderPreviewFile();
            }

            return true;
        }

        private bool CreateImagePreviewFile()
        {
            try
            {
                var image = System.Drawing.Image.FromFile(FullPath);
                image.Save(PreviewPath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
            }
            return true;
        }

        private bool CreatePlaceholderPreviewFile()
        {
            try
            {
                var image = System.Drawing.Image.FromFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "images/doc-placeholder.png"));
                image.Save(PreviewPath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            { }
            return true;
        }

        public void CreatePreviewFile()
        {
            if (!File.Exists(PreviewPath))
            {
                switch (Path.GetExtension(NameWithExtension))
                {
                    case ".pdf":
                        CreatePDFPreviewFile();
                        break;
                    case ".docx":
                        CreateDocxPreviewFile();
                        break;
                    case ".pptx":
                        CreatePPTPreviewFile();
                        break;
                    case ".JPEG":
                    case ".jpeg":
                    case ".JPG":
                    case ".jpg":
                    case ".SVG":
                    case ".svg":
                    case ".PNG":
                    case ".png":
                        CreateImagePreviewFile();
                        break;
                    default:
                        CreatePlaceholderPreviewFile(); // Default if any other file type
                        break;
                }
            }
        }
    }
}
