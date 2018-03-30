using System;
using System.IO;
using System.Threading.Tasks;

namespace Ftp2Azure.Ftp.FileSystem
{
    public interface IFile
    {
        Stream BlobStream { get; set; }
        int Read(byte[] abData, int nDataSize);
        int Write(byte[] abData, int nDataSize);
        void Close();
    }

    public interface IFileInfo
    {
        DateTimeOffset GetModifiedTime();
        long GetSize();
        string GetAttributeString();
        bool IsDirectory();
        string Path();
        bool FileObjectExists();
    }

    public interface IFileSystem
    {
        Task<IFile> OpenFile(string sPath, bool fWrite);
        Task<IFileInfo> GetFileInfo(string sPath);
        Task<IFileInfo> GetDirectoryInfo(string sPath);

        Task<string[]> GetFiles(string sDirPath);
        Task<string[]> GetDirectories(string sDirPath);

        Task<bool> DirectoryExists(string sDirPath);
        Task<bool> FileExists(string sPath);

        Task<bool> CreateDirectory(string sPath);
        Task<bool> Move(string sOldPath, string sNewPath);// file, not directory
        Task<bool> DeleteFile(string sPath);
        Task<bool> DeleteDirectory(string sPath);
        Task<bool> AppendFile(string sPath, Stream stream);

        Task Log4Upload(string sPath);// upload notification
        Task SetFileMd5(string sPath, string md5Value);// record md5 for upload files
    }

    public interface IFileSystemClassFactory
    {
        IFileSystem Create(string sUser, string sPassword);
    }
}