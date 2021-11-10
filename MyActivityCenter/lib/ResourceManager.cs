using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyActivityCenter.lib
{
    class ResourceManager
    {
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

        public List<string> SupportedDocTypes = new List<string>() { ".pdf", ".docx", ".pptx" };
        public List<string> SupportedImageTypes = new List<string>() { ".png", ".jpg", ".jpeg", ".svg" };

        private static ResourceManager _instance;

        public static ResourceManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }
            return _instance;
        }

        public List<ResourceFile> GetResources(string path)
        {
            // Get's resources in provided path
            List<ResourceFile> resFiles = new List<ResourceFile>();
            if (!Directory.Exists(path))
                return resFiles;
            var resPaths =  Directory.GetFiles(path, "*", SearchOption.AllDirectories).Where(item => !item.Contains("preview-cache") && !item.Contains("~$")).ToArray();
            foreach (string resPath in resPaths)
            {
                resFiles.Add(GetResourceFileFromPath(resPath));
            }

            return resFiles;
        }

        public ResourceFile GetResourceFileFromPath(string path)
        {
            if (!File.Exists(path))
                return null;

            // File exists. Proceed to get ResourceFile
            var resourceFile = new ResourceFile()
            {
                FullPath = path
            };            
            return resourceFile;
        }



    } 
}
