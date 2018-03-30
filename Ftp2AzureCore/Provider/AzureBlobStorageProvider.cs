using Ftp2Azure.Azure;
using Ftp2Azure.Ftp.General;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp2Azure.Provider
{
    public class StorageProviderEventArgs : EventArgs
    {
        public StorageOperation Operation;
        public StorageOperationResult Result;
    }

    public sealed class AzureBlobStorageProvider
    {
        #region Member variables

        private CloudStorageAccount _account;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private IConfiguration config;

        #endregion

        #region Construction

        public AzureBlobStorageProvider(string containerName, IConfiguration config)
        {
            this.config = config;
            Initialise(containerName).Wait();
        }

        #endregion

        #region Properties

        public bool UseHttps { get; private set; }

        public String ContainerName { private get; set; }

        #endregion

        #region IStorageProvider Members

        /// <summary>
        /// Occurs when a storage provider operation has completed.
        /// </summary>
        //public event EventHandler<StorageProviderEventArgs> StorageProviderOperationCompleted;

        #endregion

        // Initialiser method
        private async Task Initialise(string containerName)
        {
            if (String.IsNullOrEmpty(containerName))
                throw new ArgumentException("You must provide the base Container Name", "containerName");

            ContainerName = containerName;

            if (StorageProviderConfiguration.Mode == Modes.Debug)
            {
                _account = CloudStorageAccount.DevelopmentStorageAccount;
                _blobClient = _account.CreateCloudBlobClient();
                _blobClient.DefaultRequestOptions.ServerTimeout = new TimeSpan(0, 0, 0, 5);
            }
            else
            {
                _account = CloudStorageAccount.Parse(config["StorageAccount"]);
                _blobClient = _account.CreateCloudBlobClient();
                _blobClient.DefaultRequestOptions.ServerTimeout = new TimeSpan(0, 0, 0, 5);
            }

            _container = _blobClient.GetContainerReference(ContainerName);
            try
            {
                await _container.FetchAttributesAsync();
            }
            catch (StorageException)
            {
                Console.WriteLine("Information: Create new container: {0}", ContainerName);
                await _container.CreateAsync();

                // set new container's permissions
                // Create a permission policy to set the public access setting for the container. 
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

                // The public access setting explicitly specifies that the container is private,
                // so that it can't be accessed anonymously.
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;

                //Set the permission policy on the container.
                await _container.SetPermissionsAsync(containerPermissions);
            }
        }

        #region Storage operations

        public async Task<CloudBlobStream> GetWriteBlobStream(string path)
        {
            var blob = GetCloudBlockBlob(path);

            if (blob == null)
                return null;

            var stream = blob.OpenWriteAsync();
            return await stream;
        }

        public async Task<Stream> GetReadBlobStream(string path)
        {
            var blob = GetCloudBlob(path);

            if (blob == null)
                return null;

            Stream stream = await blob.OpenReadAsync();
            stream.Position = 0;

            return stream;
        }

        private CloudBlob GetCloudBlob(string path)
        {
            // convert to azure path
            string blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            // Create a reference for the filename
            // Note: won't check whether the blob exists
            CloudBlob blob = container.GetBlobReference(blobPath);
            return blob;
        }

        private CloudBlockBlob GetCloudBlockBlob(string path)
        {
            // convert to azure path
            string blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            // Create a reference for the filename
            // Note: won't check whether the blob exists
            var blob = container.GetBlockBlobReference(blobPath);
            return blob;
        }

        /// <summary>
        /// Delete the specified file from the Azure container.
        /// </summary>
        /// <param name="path">the file to be deleted</param>
        public async Task<bool> DeleteFile(string path)
        {
            if (!(await IsValidFile(path)))
                return false;

            // convert to azure path
            string blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            CloudBlob b = container.GetBlobReference(blobPath);
            if (b != null)
            {
                await b.DeleteAsync();
            }
            else
            {
                Console.WriteLine("Error: Get blob reference \"{0}\" failed", path);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Delete the specified directory from the Azure container.
        /// </summary>
        /// <param name="path">the directory path</param>
        public async Task<bool> DeleteDirectory(string path)
        {
            if (!(await IsValidDirectory(path)))
                return false;

            // cannot delete root directory
            if (path == "/")
                return false;

            var dirPath = GetFullPath(path);

            var container = _container;
            rootFix(ref container, ref dirPath);

            //TODO loop with contiunation token,
            IEnumerable<IListBlobItem> allFiles = (await
                container.ListBlobsSegmentedAsync(dirPath, true, BlobListingDetails.Metadata, 500000, null, null, null)).Results;

            await Task.WhenAll(allFiles.OfType<CloudBlob>().Select(async file => await file.DeleteIfExistsAsync()));

            if (dirPath == "")
                await container.DeleteAsync();

            return true;
        }

        /// <summary>
        /// Retrieves the object from the storage provider
        /// </summary>
        /// <param name="path">the file/dir path in the FtpServer</param>
        /// <param name="isDirectory">whether path is a directory</param>
        /// <returns>AzureCloudFile</returns>
        /// <exception cref="FileNotFoundException">Throws a FileNotFoundException if the blob path is not found on the provider.</exception>
        public async Task<AzureCloudFile> GetBlobInfo(string path, bool isDirectory)
        {
            // check parameter
            if (path == null || path == "" || path[0] != '/')
                return null;

            // get the info of root directory
            if ((path == "/") && isDirectory)
                return new AzureCloudFile
                {
                    Uri = _container.Uri,
                    FtpPath = path,
                    IsDirectory = true,
                    Size = 1,
                    LastModified = DateTime.Now
                };

            if (path.StartsWith("/$root")) // Container Info
            {
                var containerName = path.Remove(0, 7);
                var container = _blobClient.GetContainerReference(containerName);
                await container.FetchAttributesAsync();

                return new AzureCloudFile
                {
                    Uri = container.Uri,
                    FtpPath = path,
                    IsDirectory = true,
                    // default value for size and modify time of directories
                    Size = 1,
                    LastModified = container.Properties.LastModified.Value
                };
            }

            // convert to azure path
            string blobPath = path.ToAzurePath();

            var o = new AzureCloudFile();

            try
            {
                var container = _container;
                rootFix(ref container, ref blobPath);

                if (isDirectory)
                {
                    CloudBlobDirectory bDir = container.GetDirectoryReference(blobPath);
                    // check whether directory exists

                    if ((await bDir.ListBlobsSegmentedAsync(null)).Results.Count() == 0)
                        throw new StorageException();
                    o = new AzureCloudFile
                    {
                        Uri = bDir.Uri,
                        FtpPath = path,
                        IsDirectory = true,
                        // default value for size and modify time of directories
                        Size = 1,
                        LastModified = DateTime.Now
                    };
                }
                else
                {
                    CloudBlob b = container.GetBlobReference(blobPath);
                    await b.FetchAttributesAsync();
                    o = new AzureCloudFile
                    {
                        Uri = b.Uri,
                        LastModified = b.Properties.LastModified.Value,
                        Size = b.Properties.Length,
                        FtpPath = path,
                        IsDirectory = false
                    };
                }

            }
            catch (StorageException)
            {
                Console.WriteLine("Error: Get blob {0} failed", path);
                return null;
            }

            return o;
        }

        /// <summary>
        /// Gets the containers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CloudBlobContainer>> GetContainerListing()
        {
            var list = await _blobClient.ListContainersSegmentedAsync(null);
            return list.Results;
        }

        /// <summary>
        /// Gets the directory under the directory
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns></returns>
        public async Task<IEnumerable<CloudBlobDirectory>> GetDirectoryListing(string path)
        {
            // Get the full path of directory
            string prefix = GetFullPath(path);

            var results = await _blobClient.ListBlobsSegmentedAsync(prefix, null);

            return results.Results.OfType<CloudBlobDirectory>();
        }

        /// <summary>
        /// List the files under the directory
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns></returns>
        public async Task<IEnumerable<CloudBlob>> GetFileListing(string path)
        {
            // Get the full path of directory
            string prefix = GetFullPath(path);

            var results = await _blobClient.ListBlobsSegmentedAsync(prefix, null);

            return results.Results.OfType<CloudBlob>();
        }

        /// <summary>
        /// Renames the specified object by copying the original to a new path and deleting the original.
        /// </summary>
        /// <param name="oldPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns></returns>
        public async Task<StorageOperationResult> Rename(string oldPath, string newPath)
        {
            var oldBlobPath = oldPath.ToAzurePath();
            var newBlobPath = newPath.ToAzurePath();

            var containerOld = _container;
            rootFix(ref containerOld, ref oldBlobPath);
            var containerNew = _container;
            rootFix(ref containerNew, ref newBlobPath);

            var oldBlob = containerOld.GetBlockBlobReference(oldBlobPath);
            var newBlob = containerNew.GetBlockBlobReference(newBlobPath);

            // Check if the original path exists on the provider.
            if (!(await IsValidFile(oldPath)))
            {
                throw new FileNotFoundException(
                    "The path supplied does not exist on the storage provider", oldPath);
            }

            await newBlob.StartCopyAsync(oldBlob);

            try
            {
                await newBlob.FetchAttributesAsync();
                await oldBlob.DeleteAsync();
                return StorageOperationResult.Completed;
            }
            catch (StorageException)
            {
                throw;
            }
        }

        public async Task<bool> CreateDirectory(string path)
        {
            path = path.ToAzurePath();

            string blobPath = string.Concat(path, "required.req");

            var container = _container;
            rootFix(ref container, ref blobPath);
            if (ContainerName == "$root")
                await container.CreateIfNotExistsAsync();

            try
            {
                var blob = container.GetBlockBlobReference(blobPath);

                string message = "#REQUIRED: At least one file is required to be present in this folder.";
                await blob.UploadTextAsync(message);

                BlobProperties props = blob.Properties;
                props.ContentType = "text/text";
                await blob.SetPropertiesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified path is a valid blob folder.
        /// </summary>
        /// <param name="path">a directory path, final char is '/'</param>
        /// <returns></returns>
        public async Task<bool> IsValidDirectory(string path)
        {
            if (path == null)
                return false;

            // Important, when dirPath = "/", the behind HasDirectory(dirPath) will throw exceptions
            if (path == "/")
                return true;

            // error check
            if (!path.EndsWith(@"/"))
            {
                Console.WriteLine("Error: Invalid parameter {0} for function IsValidDirectory", path);
                return false;
            }

            // remove the first '/' char
            string dirPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref dirPath);

            if (dirPath == "") return await container.ExistsAsync();

            // get reference
            CloudBlobDirectory blobDirectory = container.GetDirectoryReference(dirPath);

            // non-exist blobDirectory won't contain blobs
            if ((await blobDirectory.ListBlobsSegmentedAsync(null)).Results.Count() == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks whether the specified path is a valid .
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<bool> IsValidFile(string path)
        {
            if (path == null)
                return false;

            // error check
            if (path.EndsWith(@"/"))
            {
                Console.WriteLine("Error: Invalid parameter {0} for function IsValidFile", path);
                return false;
            }

            // remove the first '/' char
            string blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            CloudBlob blob = container.GetBlobReference(blobPath);

            return await blob.ExistsAsync();
        }

        /// <summary>
        /// read bytes from the stream and append the content to an existed file
        /// </summary>
        /// <param name="blobPath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<bool> AppendFileFromStream(string path, Stream stream)
        {
            var blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            CloudBlob rawBlob = container.GetBlobReference(blobPath);
            if (rawBlob.Properties.BlobType == BlobType.PageBlob)
            {
                return false; //for page blob, append is not supported
            }

            CloudBlockBlob blob = container.GetBlockBlobReference(blobPath);

            // store the block id of this blob
            var blockList = new List<string>();

            try
            {
                foreach (var block in await blob.DownloadBlockListAsync())
                {
                    blockList.Add(block.Name);
                }
            }
            catch (StorageException)
            {
                // do nothing, this may happen when blob doesn't exist
            }

            const int blockSize = 4 * 1024 * 1024; // 4M - block size
            byte[] buffer = new byte[blockSize];
            // append file
            try
            {
                int nRead = 0;
                while (nRead < blockSize)
                {
                    int actualRead = stream.Read(buffer, nRead, blockSize - nRead);
                    if (actualRead <= 0) // stream end
                    {
                        //put last block & break
                        string strBlockId = GetBlockID(blockList);
                        await blob.PutBlockAsync(strBlockId, new System.IO.MemoryStream(buffer, 0, nRead), null);
                        blockList.Add(strBlockId);
                        break;
                    }
                    else if (actualRead == (blockSize - nRead))// buffer full
                    {
                        //put this block
                        string strBlockId = GetBlockID(blockList);
                        await blob.PutBlockAsync(strBlockId, new System.IO.MemoryStream(buffer), null);
                        blockList.Add(strBlockId);
                        nRead = 0;
                        continue;
                    }
                    nRead += actualRead;
                }
                var ext = Path.GetExtension(path);
                blob.Properties.ContentType = MimeTypes.Core.MimeTypeMap.GetMimeType(ext);
                await blob.SetPropertiesAsync();
            }
            catch (StorageException)
            {
                // blob.PutBlock error
                return false;
            }

            // put block list
            await blob.PutBlockListAsync(blockList);

            return true;
        }

        /// <summary>
        /// After successfully upload a new file, user Azure queue to record it
        /// </summary>
        /// <param name="filePath">the path of the new file</param>
        public async Task UploadNotification(string filePath)
        {
            if (!StorageProviderConfiguration.QueueNotification)
                return;

            // Create the queue client
            CloudQueueClient queueClient = _account.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("ftp2azure-queue");

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            // Get the new blob's URI
            // remove the first '/' char
            string fileBlobPath = filePath.ToAzurePath();
            CloudBlob blob = _container.GetBlobReference(fileBlobPath);

            // Create a message and add it into the queue
            CloudQueueMessage message = new CloudQueueMessage(string.Format("User uploaded blob: {0}", blob.Uri));
            await queue.AddMessageAsync(message);
        }

        /// <summary>
        /// Set blob ContentMD5
        /// </summary>
        /// <param name="path">blob path</param>
        /// <param name="md5Value"></param>
        public async Task SetBlobMd5(string path, string md5Value)
        {
            // get the blob
            // remove the first '/' char
            string blobPath = path.ToAzurePath();

            var container = _container;
            rootFix(ref container, ref blobPath);

            CloudBlob blob = container.GetBlobReference(blobPath);

            await blob.FetchAttributesAsync();

            blob.Properties.ContentMD5 = md5Value;

            await blob.SetPropertiesAsync();
        }

        #endregion

        #region "Helper methods"

        /// <summary>
        /// Get the full path (as in URI) of a blob folder or file
        /// </summary>
        /// <param name="path">a folder path or a file path, absolute path</param>
        /// <returns></returns>
        private string GetFullPath(string path)
        {
            var result = ContainerName + path;

            if (result.StartsWith("$root/"))
                result = result.Remove(0, 6);

            return result;
        }

        private string GetBlockID(List<string> currentIds)
        {
            string blockID = null;

            while (true)
            {
                string tempStr = Convert.ToBase64String(Encoding.ASCII.GetBytes(DateTime.Now.ToBinary().ToString()));
                int idLength = (currentIds.Count() == 0) ? 64 : currentIds[0].Length;
                tempStr = TextHelpers.RightAlignString(tempStr, idLength, 'A');
                bool sameId = false;
                foreach (var id in currentIds)
                {
                    if (id == tempStr)
                    {
                        sameId = true;
                        break;
                    }
                }
                if (!sameId)
                {
                    blockID = tempStr;
                    break;
                }
            }

            return blockID;
        }

        private async Task _GetBlobMd5(string filePath)
        {
            string fileBlobPath = filePath.ToAzurePath();
            CloudBlob blob = _container.GetBlobReference(fileBlobPath);

            await blob.FetchAttributesAsync();

            Console.WriteLine("GetMd5#" + blob.Properties.ContentMD5);
        }

        private void rootFix(ref CloudBlobContainer container, ref string blobPath)
        {
            if (ContainerName != "$root") return;

            var slashIdx = blobPath.IndexOf('/');
            if (slashIdx > -1)
            {
                container = _blobClient.GetContainerReference(blobPath.Substring(0, slashIdx));
                blobPath = blobPath.Substring(slashIdx + 1);
            }
            else
                container = _blobClient.GetRootContainerReference();
        }

        #endregion
    }
}