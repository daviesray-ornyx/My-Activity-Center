using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.Office.Core;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;

namespace MyActivityCenter.lib
{
    public class ResourceFile
    {
        private GhostscriptVersionInfo _version = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);


        public string FullPath { get; set; }

        public string _name;
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name))
                    _name = Path.GetFileNameWithoutExtension(FullPath);
                return _name;
            }
            set
            {
                if (_name == value)
                    return;
                _name = value;
            }
        }

        public string _nameWithExtension;
        public string NameWithExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_nameWithExtension))
                    _nameWithExtension = Path.GetFileName(FullPath);
                return _nameWithExtension;
            }
            set
            {
                if (_nameWithExtension == value)
                    return;
                _nameWithExtension = value;
            }
        }

        public string _logo { get; set; }
        public string Logo
        {
            get
            {
                if (string.IsNullOrEmpty(_logo))
                {
                    var docType = Path.GetExtension(NameWithExtension);
                    if (ResourceManager.GetInstance().SupportedDocTypes.Contains(docType))
                    {
                        _logo = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "images", "logo" + docType.Replace(".", "-") + ".png");
                    }
                    else if (ResourceManager.GetInstance().SupportedImageTypes.Contains(docType))
                    {
                        _logo = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "images", "logo-image.png");
                    }
                    else
                        _logo = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "images", "logo-general.png");
                }
                return _logo;
            }
        }
        
        private string _previewPath;
        public string PreviewPath {
            get
            {
                if (String.IsNullOrEmpty(_previewPath))
                {
                    var directoryName = Path.Combine(Path.GetDirectoryName(FullPath), @"preview-cache");
                    // Create directory if it does not exist
                    Directory.CreateDirectory(directoryName);
                    _previewPath = Path.Combine(directoryName, Name + ".jpeg");
                    CreatePreviewFile(_previewPath);
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
              

        public ResourceFile()
        {

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

        public static void KillProcessBlankMainTitle(string name) // WINWORD
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {

                if (Process.GetCurrentProcess().Id == clsProcess.Id)
                    continue;
                if (clsProcess.ProcessName.Equals(name))
                {
                    if (clsProcess.MainWindowTitle.Equals(""))
                        clsProcess.Kill();
                }
            }
        }

        private void CloseDoc(Microsoft.Office.Interop.Word.Document doc)
        {
            try
            {
                if (doc != null)
                {
                    object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                    object originalFormat = Microsoft.Office.Interop.Word.WdOriginalFormat.wdOriginalDocumentFormat;
                    object routeDocument = false;
                    doc.Close(ref saveOption, ref originalFormat, ref routeDocument);
                    KillProcessBlankMainTitle("WINWORD");
                }
            }
            catch (Exception ex)
            {
                // Do nothing for now
            }
            
            
        }

        public bool CreateDocxPreviewFile()
        {
            Document doc = null;
            try
            {
                doc = ResourceManager.GetInstance().MSWordApp.Documents.Open(FullPath, ReadOnly: true, Visible: false);
                doc.ShowGrammaticalErrors = false;
                doc.ShowRevisions = false;
                doc.ShowSpellingErrors = false;

                foreach (Microsoft.Office.Interop.Word.Window win in doc.Windows)
                {
                    foreach (Microsoft.Office.Interop.Word.Pane pane in win.Panes)
                    {
                        if (pane.Pages.Count > 0)
                        {
                            Microsoft.Office.Interop.Word.Page page = pane.Pages[1];
                            var bits = page.EnhMetaFileBits;
                            using (var memoryStream = new MemoryStream((byte[])(bits)))
                            {
                                var image = System.Drawing.Image.FromStream(memoryStream);
                                image.Save(PreviewPath, ImageFormat.Jpeg);
                            }
                        }
                        break;
                    }
                    break;
                }
                CloseDoc(doc);
                
            }
            catch (System.Exception ex)
            {
                CreatePlaceholderPreviewFile();
            }
            finally
            {
                CloseDoc(doc);

                if (ResourceManager.GetInstance().MSPPTApp != null)
                {
                    ResourceManager.GetInstance().MSPPTApp.Quit();
                }
            }

            return true;
        }
                
        public bool CreatePPTPreviewFile()
        {
            try
            {
                var pptPresentation = ResourceManager.GetInstance().MSPPTApp.Presentations.Open(FullPath, MsoTriState.msoFalse, MsoTriState.msoFalse, WithWindow: MsoTriState.msoFalse);

                pptPresentation.Slides[1].Export(PreviewPath, "jpg", Int32.Parse(pptPresentation.SlideMaster.Width.ToString()), Int32.Parse(pptPresentation.SlideMaster.Height.ToString()));

                pptPresentation.Close();
            }
            catch (System.Exception ex)
            {
                CreatePlaceholderPreviewFile();
            }

            return true;
        }

        public bool CreateImagePreviewFile()
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

        public bool CreatePlaceholderPreviewFile()
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

        public void CreatePreviewFile(string previewPath)
        {
            if (!File.Exists(previewPath))
            {
                switch (Path.GetExtension(NameWithExtension).ToLower())
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
                    case ".jpeg":
                    case ".jpg":
                    case ".svg":
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
