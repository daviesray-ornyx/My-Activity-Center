using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using MyActivityCenter.Properties;

namespace MyActivityCenter.lib
{
    class GoogleDriveService
    {
        private GoogleDriveService() { }
        private static GoogleDriveService _instance;

        private Settings _currSettings;
        public Settings CurrSettings
        {
            get
            {
                if (_currSettings == null)
                    _currSettings = SettingsManager.GetInstance().GetSettings();
                return _currSettings;
            }
            set
            {
                if (value == _currSettings)
                    return;
                _currSettings = value;
            }
        }

        static string[] Scopes = { DriveService.Scope.DriveReadonly };

        public static GoogleDriveService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GoogleDriveService();
            }
            return _instance;
        }

        private Google.Apis.Drive.v3.Data.File GetSharedFolder(DriveService service, string name)
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.SupportsAllDrives = true;
            listRequest.IncludeItemsFromAllDrives = true;
            listRequest.Q = "name = '" + name + "'";
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            if (files.Count > 0)
            {
                return files[0];
            }
            return null;
        }

        private Google.Apis.Drive.v3.Data.File GetLinkedResourcesFile(DriveService service, string folderId)
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.SupportsAllDrives = true;
            listRequest.IncludeItemsFromAllDrives = true;
            listRequest.Q = "'" + folderId + "' in parents and mimeType != 'application/vnd.google-apps.folder'";
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, webContentLink, modifiedTime)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            if (files.Count > 0)
            {
                return files[0];
            }
            return null;
        }

        private string[] picsMimeTypes = { "image/jpeg", "image/png", "image/svg+xml" };
        private string[] docMimeTypes = { "application/pdf" };
        
        private IList<Google.Apis.Drive.v3.Data.File> GetChildItems(DriveService service, string folderId) {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.SupportsAllDrives = true;
            listRequest.IncludeItemsFromAllDrives = true;
            listRequest.Q = "'" + folderId + "' in parents";
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, webContentLink, modifiedTime)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> folders = listRequest.Execute()
                .Files;
            return folders;
        }

        private BusyIndicatorWindow _busyIndicator;
        public BusyIndicatorWindow BusyIndicator
        {
            get {
                if (_busyIndicator == null)
                    _busyIndicator = new BusyIndicatorWindow();
                return _busyIndicator;
            }
            set {
                if (value == _busyIndicator)
                    return;
                _busyIndicator = value;
            }
        }


        public async Task ProcessFolderAsync(DriveService service, string folderId, bool isRootFolder, string localFolderPath)
        {
            // Create folder if not exists
            // Try setting directory
            if (!Directory.Exists(localFolderPath))
            {
                Directory.CreateDirectory(localFolderPath); // Create directory if not exists
            }

            // get all subfolders
            var childItems = GetChildItems(service, folderId);
            foreach (var item in childItems)
            {
                // retrieve onlineResources file if isRootFolder
                if (isRootFolder)
                {
                    var linkedResourcesfile = GetLinkedResourcesFile(service, folderId); // Recheck for this
                    if (linkedResourcesfile != null && (!System.IO.File.Exists(CurrSettings.OnlineResourcesFilePath) || (new FileInfo(CurrSettings.OnlineResourcesFilePath)).LastWriteTime < linkedResourcesfile.ModifiedTime))
                    {
                        using (var stream = new FileStream(CurrSettings.OnlineResourcesFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await service.Files.Get(linkedResourcesfile.Id).DownloadAsync(stream);
                        }
                    }
                }

                if (item.MimeType == "application/vnd.google-apps.folder")
                {
                    // process subFolders within
                    await ProcessFolderAsync(service, item.Id, false, Path.Combine(localFolderPath, item.Name));
                }
                else if (!System.IO.File.Exists(Path.Combine(localFolderPath, item.Name)) || (new FileInfo(localFolderPath)).CreationTime < item.CreatedTime) // Only add files that do not exist in the 
                {
                    
                    // Process files
                    using (var stream = new FileStream(Path.Combine(localFolderPath, item.Name), FileMode.Create, FileAccess.Write))
                    {
                        await service.Files.Get(item.Id).DownloadAsync(stream);
                    }
                }             
            }
        }

        public async void ProcessDriveAsync()
        {
            // Show processing indicator
            BusyIndicator.Show();
            UserCredential credential;

            using (var stream =
                new FileStream(Resources.google_client_secret_file_path, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(Resources.goolge_credential_path, true)).Result;
                Console.WriteLine("Credential file saved to: " + Resources.goolge_credential_path);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Resources.app_name,
            });
            var sharedFolder = GetSharedFolder(service, "My Activity Center");
            if (sharedFolder == null)
            {
                // Ask user to update shared folder list
                if(CurrSettings != null && !String.IsNullOrEmpty(CurrSettings.SharedGoogleFolderPath))
                {
                    Process.Start(@"https://drive.google.com/drive/folders/1NJsFrhDQNq-y70UhPf7VpD4dwoIXH4mN?usp=sharing");
                }
            }
            else
            {
                await ProcessFolderAsync(service, sharedFolder.Id, true, CurrSettings.LocalResourcesFolder);
            }

            // Hide processing indicator
            BusyIndicator.Close();
        }
    }
}
