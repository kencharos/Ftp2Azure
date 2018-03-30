using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.General;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// base class for RMD & XRMD command handlers
    /// </summary>
    internal class RemoveDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected RemoveDirectoryCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));

            string dirToRemove = GetPath(FileNameHelpers.AppendDirTag(sMessage));

            // check whether directory exists
            if (!await ConnectionObject.FileSystemObject.DirectoryExists(dirToRemove))
            {
                return GetMessage(550, string.Format("Directory \"{0}\" does not exist", dirToRemove));
            }

            // can not delete root directory
            if (dirToRemove == "/")
            {
                return GetMessage(553, "Can not remove root directory");
            }

            // delete directory
            if (await ConnectionObject.FileSystemObject.DeleteDirectory(dirToRemove))
            {
                return GetMessage(250, string.Format("{0} successful.", Command));
            }
            else
            {
                return GetMessage(550, string.Format("Couldn't remove directory ({0}).", dirToRemove));
            }
        }
    }
}