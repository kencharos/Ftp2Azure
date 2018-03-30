using Ftp2Azure.Ftp.FileSystem;
using Ftp2Azure.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ftp2Azure.Azure
{
    public class AzureFileSystem : IFileSystem
    {
        private readonly AzureBlobStorageProvider _provider;
        private string _containerName;

        // Constructor
        public AzureFileSystem(string containerName, IConfiguration config)
        {
            // Set container name (if none specified, specify the development container default)
            _containerName = !string.IsNullOrEmpty(containerName) ? containerName : "DevelopmentContainer";
            _provider = new AzureBlobStorageProvider(_containerName, config);
        }

        #region Implementation of IFileSystem

        public async Task<IFile> OpenFile(string sPath, bool fWrite)
        {
            var f = new AzureFile();
            if (fWrite == true)
            {
                f.BlobStream = await _provider.GetWriteBlobStream(sPath);
            }
            else
            {
                f.BlobStream = await _provider.GetReadBlobStream(sPath);
            }

            if (f.BlobStream == null)
                return null;

            return f;
        }

        public async Task<IFileInfo> GetFileInfo(string sPath)
        {
            AzureCloudFile file = await _provider.GetBlobInfo(sPath, false);

            return new AzureFileInfo(file);
        }

        public async Task<IFileInfo> GetDirectoryInfo(string sDirPath)
        {
            AzureCloudFile dir = await _provider.GetBlobInfo(sDirPath, true);

            return new AzureFileInfo(dir);
        }

        /// <summary>
        /// Get the filename list in the directory 
        /// </summary>
        /// <param name="sDirPath">directory path</param>
        /// <returns>an arry of filenames</returns>
        public async Task<string[]> GetFiles(string sDirPath)
        {
            IEnumerable<CloudBlob> files = await _provider.GetFileListing(sDirPath);
            string[] result = files.Select(r => r.Uri.AbsolutePath.ToString()).ToArray().ToFtpPath(sDirPath);
            return result;
        }

        /// <summary>
        /// Get the directory name list in the directory 
        /// </summary>
        /// <param name="sDirPath">directory path</param>
        /// <returns>an arry of directorynames</returns>
        public async Task<string[]> GetDirectories(string sDirPath)
        {
            string[] result;

            if (_containerName + sDirPath == "$root/")
                result = (await _provider.GetContainerListing())
                    .Select(r => r.Uri.AbsolutePath.ToString())
                    .Where(s => s != "/$root").Select(s => "/$root" + s + "/")
                    .ToArray();
            else
                result = (await _provider.GetDirectoryListing(sDirPath))
                    .Select(r => r.Uri.AbsolutePath.ToString())
                    .ToArray().ToFtpPath(sDirPath);

            return result;
        }

        /// <summary>
        /// check if the directory exists
        /// </summary>
        /// <param name="sPath">the directory name, final char is '/'</param>
        /// <returns></returns>
        public async Task<bool> DirectoryExists(string sDirPath)
        {
            return await _provider.IsValidDirectory(sDirPath);
        }

        /// <summary>
        /// check if the file exists
        /// </summary>
        /// <param name="sPath">the file name</param>
        /// <returns></returns>
        public async Task<bool> FileExists(string sPath)
        {
            return await _provider.IsValidFile(sPath);
        }

        public async Task<bool> CreateDirectory(string sPath)
        {
            return await _provider.CreateDirectory(sPath);
        }

        public async Task<bool> Move(string sOldPath, string sNewPath)
        {
            return await _provider.Rename(sOldPath, sNewPath) == StorageOperationResult.Completed;
        }

        public async Task<bool> DeleteFile(string sPath)
        {
            return await _provider.DeleteFile(sPath);
        }

        public async Task<bool> DeleteDirectory(string sPath)
        {
            return await _provider.DeleteDirectory(sPath);
        }

        public async Task<bool> AppendFile(string sPath, Stream stream)
        {
            return await _provider.AppendFileFromStream(sPath, stream);
        }

        public async Task Log4Upload(string sPath)
        {
            await _provider.UploadNotification(sPath);
        }

        public async Task SetFileMd5(string sPath, string md5Value)
        {
            await _provider.SetBlobMd5(sPath, md5Value);
        }

        #endregion
    }
}
