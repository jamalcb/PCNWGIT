using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using PCNW.Models.ContractModels;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace PCNW.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public int GetMemberId(string MemberName)
        {
            int memberId = 1;
            return memberId;
        }
        /// <summary>
        /// To Get Ip Address of client
        /// </summary>
        /// <returns></returns>
        public string GetIPAddressOfClient()
        {
            string ipAddress = string.Empty;
            IPAddress ip = Request.HttpContext.Connection.RemoteIpAddress;
            if (ip == null)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = Dns.GetHostEntry(ip).AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);
                }
                ipAddress = ip.ToString();
            }
            return ipAddress;
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="nameId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ToMountedDrive(string nameId)
        {
            string url = @"z:\2022\10\" + nameId;
            Process.Start("explorer.exe", @"z:\2022\10\" + nameId);
            return RedirectToAction("Dashboard", "member");

        }
        /// <summary>
        /// Create unique path name in case of duplicate file (Copy Centers, Preview/ print order form)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string CreatePath(string path)
        {
            string today = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            string combinedPath = Path.Combine(path, today);
            if (!string.IsNullOrEmpty(combinedPath))
            {
                if (Directory.Exists(combinedPath))
                {
                    CreatePath(path);
                }

            }
            return combinedPath;
        }
        /// <summary>
        /// To create unique zip path in root folder from Preview/Print order form
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string CreateZipPath(string path)
        {
            string today = DateTime.Now.ToString("yyyyMMddHHmmssFFF") + ".zip";
            string combinedPath = Path.Combine(path, today);
            if (!string.IsNullOrEmpty(combinedPath))
            {
                if (Directory.Exists(combinedPath))
                {
                    CreateZipPath(path);
                }

            }
            return combinedPath;
        }

        public async Task<List<string>> GetListOfS3Content(string folder, string subFolder)
        {
            string key = folder + subFolder;
            List<string> addenda = new List<string>();
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                var request1 = new GetObjectMetadataRequest();
                //request1.Key = key;
                request1.Key = key;
                request1.BucketName = "contractor-aws";
                IAmazonS3 client =
                new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1);
                request.BucketName = "contractor-aws";
                //request.Prefix = "/";
                request.Marker = key;
                ListObjectsResponse response = await client.ListObjectsAsync(request);
                addenda = response.S3Objects.Where(x => x.Key.Contains(key)).Select(x => x.Key).ToList();
            }
            catch (Amazon.S3.AmazonS3Exception ex)
            {

            }
            return addenda;
        }
        /// <summary>
        /// Get Addenda folder information 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public async Task<List<AddendaFileInfo>> GetListOfS3ContentAddenda(string folder, string subFolder)
        {
            string key = folder + subFolder;
            List<AddendaFileInfo> addenda = new();
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                var request1 = new GetObjectMetadataRequest();
                //request1.Key = key;
                request1.Key = key;
                request1.BucketName = "contractor-aws";
                IAmazonS3 client =
                new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1);
                request.BucketName = "contractor-aws";
                //request.Prefix = "/";
                request.Marker = key;
                ListObjectsResponse response = await client.ListObjectsAsync(request);
                addenda = response.S3Objects.Where(x => x.Key.Contains(key)).Select(x => new AddendaFileInfo { PathInfo = x.Key, Size = x.Size }).ToList();
            }
            catch (Amazon.S3.AmazonS3Exception ex)
            {

            }
            return addenda;
        }

        /// <summary>
        /// Create folder when new project is created from home/sentprojectfiles, StaffAccount/EditProjectinfo, Member/PostProjectHerw 
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task CreateFolder(string folderName)
        {
            var folderKey = folderName + "/"; //end the folder name with "/"

            var request = new PutObjectRequest();
            request.BucketName = "contractor-aws";
            //request.WithBucketName(bucketName);
            IAmazonS3 client =
            new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S",
            RegionEndpoint.USEast1);
            request.StorageClass = S3StorageClass.Standard;
            request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.None;

            //request.CannedACL = S3CannedACL.BucketOwnerFullControl;
            request.Key = folderKey;
            request.ContentBody = string.Empty;
            //request.WithKey(folderKey);
            //request.WithContentBody(string.Empty);
            PutObjectResponse response = await client.PutObjectAsync(request);
            //S3Response response = m_S3Client.PutObject(request);

        }

        private void LocalCreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Get List of addenda files and folder from AWS S3
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<string>> GetListOfAddenda(string key)
        {
            List<string> addenda = new List<string>();
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                var request1 = new GetObjectMetadataRequest();
                //request1.Key = key;
                request1.Key = key;
                request1.BucketName = "contractor-aws";
                IAmazonS3 client =
                new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1);
                request.BucketName = "contractor-aws";
                //request.Prefix = "/";
                request.Marker = key;
                ListObjectsResponse response = await client.ListObjectsAsync(request);
                addenda = response.S3Objects.Where(x => x.Key.Contains(key)).Select(x => x.Key).ToList();
                addenda = await GetAllData(addenda, response.NextMarker, client, request, response);
            }
            catch (Amazon.S3.AmazonS3Exception ex)
            {

            }
            return addenda;
        }
        /// <summary>
        /// Get List of all addenda files and folder from AWS S3 recursively
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Marker"></param>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAllData(List<string> data, string Marker, IAmazonS3 client, ListObjectsRequest request, ListObjectsResponse response)
        {

            if (Marker == null)
            {
                return data;
            }
            else
            {
                List<string> result = new List<string>();
                request.Marker = Marker;
                response = await client.ListObjectsAsync(request);
                result = response.S3Objects.Select(x => x.Key).ToList();
                data.AddRange(result);
                if (response.NextMarker == null)
                {
                    return data;
                }
                {
                    return await GetAllData(data, response.NextMarker, client, request, response);
                }
            }
        }
        /// <summary>
        /// Download all S3 folder content as zip
        /// </summary>
        /// <param name="DownLoadPath"></param>
        /// <param name="rootPath"></param>
        /// <param name="Zip"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadDirectoryAsync(string DownLoadPath, string rootPath, string Zip, string type,string sourcepath)
        {
          
            // Create a temporary directory to store downloaded files
            string tempDownloadPath = Path.Combine(DownLoadPath, "TempDownload");
            Directory.CreateDirectory(tempDownloadPath);

            // Copy files from sourcePath to tempDownloadPath
            CopyFiles(sourcepath, tempDownloadPath);

            // Create a ZIP file from the temporary directory
            string zipDownloadFilePath = Path.Combine(DownLoadPath, $"{Zip}_{type}.zip");
            ZipFile.CreateFromDirectory(tempDownloadPath, zipDownloadFilePath);

            // Read the ZIP file into a byte array
            byte[] zipBytes = System.IO.File.ReadAllBytes(zipDownloadFilePath);

            // Delete the temporary ZIP file and directory
            System.IO.File.Delete(zipDownloadFilePath);
            Directory.Delete(tempDownloadPath, true);

            return zipBytes;
        }

        // Method to copy files from source directory to destination directory
        private void CopyFiles(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"Source directory '{sourceDir}' not found.");
            }

            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                System.IO.File.Copy(file, destFile, true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(targetDir, dirName);
                CopyFiles(dir, destDir);
            }
        }

        /// <summary>
        /// Download S3 folder content as zip
        /// </summary>
        /// <param name="folderKey"></param>
        /// <returns></returns>
        public async Task<MemoryStream> DownloadS3FolderAsZip(string folderKey)
        {
            using var client = new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1); // Replace with your desired region

            try
            {
                using var zipMemoryStream = new MemoryStream();
                var request = new ListObjectsV2Request
                {
                    BucketName = "contractor-aws",
                    Prefix = folderKey,
                    Delimiter = "/"
                };
                var response = await client.ListObjectsV2Async(request);
                using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var entry in response.S3Objects)
                    {
                        if (entry.Key.EndsWith("/")) // Skip folders
                            continue;

                        var getObjectRequest = new GetObjectRequest
                        {
                            BucketName = "contractor-aws",
                            Key = entry.Key
                        };

                        using (var getObjectResponse = await client.GetObjectAsync(getObjectRequest))
                        using (var responseStream = getObjectResponse.ResponseStream)
                        {
                            var entryPath = entry.Key.Replace(folderKey, "").TrimStart('/');
                            var zipEntry = zipArchive.CreateEntry(entryPath);

                            using (var zipEntryStream = zipEntry.Open())
                            {
                                await responseStream.CopyToAsync(zipEntryStream);
                            }
                        }
                    }
                }
                zipMemoryStream.Position = 0;
                var memoryStreamCopy = new MemoryStream(zipMemoryStream.ToArray());
                return memoryStreamCopy;

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
