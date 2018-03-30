using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.FileSystem;
using Ftp2Azure.Ftp.General;
using Ftp2Azure.General;
using System.Text;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// Base class for MLST & MLSD command handlers
    /// </summary>
    internal class MlsxCommandHandlerBase : FtpCommandHandler
    {
        protected MlsxCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected string GenerateEntry(IFileInfo info)
        {
            StringBuilder entry = new StringBuilder();

            bool isDirectory = info.IsDirectory();

            if (isDirectory)
            {
                entry.Append("Type=dir; ");
                string dirName = FileNameHelpers.GetDirectoryName(info.Path());
                entry.Append(dirName);
            }
            else
            {
                entry.Append(string.Format("Type=file;Size={0};Modify={1}; ", info.GetSize(), info.GetModifiedTime().ToString("yyyyMMddHHmmss")));
                entry.Append(FileNameHelpers.GetFileName(info.Path()));
            }

            return entry.ToString();
        }

    }
}