using Ftp2Azure.General;
using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.General;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// STAT command handler
    /// if the parameter is empty, return status message of this ftp server
    /// otherwise, work as LIST commmand
    /// </summary>
    internal class StatCommandHandler : ListCommandHandlerBase
    {
        public StatCommandHandler(FtpConnectionObject connectionObject)
            : base("STAT", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            if (sMessage == "")
            {
                return GetMessage(211, "Server status: OK");
            }

            // if no parameter is given, STAT works as LIST
            // but won't use data connection
            string[] asFiles = null;
            string[] asDirectories = null;

            // Get the file/dir to list
            string targetToList = GetPath(sMessage);

            // checks the file/dir name
            if (!FileNameHelpers.IsValid(targetToList))
            {
                return GetMessage(501, string.Format("\"{0}\" is not a valid file/directory name", sMessage));
            }

            // two vars indicating different list results
            bool targetIsFile = false;
            bool targetIsDir = false;

            // targetToList ends with '/', must be a directory
            if (targetToList.EndsWith(@"/"))
            {
                targetIsFile = false;
                if (await ConnectionObject.FileSystemObject.DirectoryExists(targetToList))
                    targetIsDir = true;
            }
            else
            {
                // check whether the target to list is a directory
                if (await ConnectionObject.FileSystemObject.DirectoryExists(FileNameHelpers.AppendDirTag(targetToList)))
                    targetIsDir = true;
                // check whether the target to list is a file
                if (await ConnectionObject.FileSystemObject.FileExists(targetToList))
                    targetIsFile = true;
            }

            if (targetIsFile)
            {
                asFiles = new string[1] { targetToList };
                if (targetIsDir)
                    asDirectories = new string[1] { FileNameHelpers.AppendDirTag(targetToList) };
            }
            // list a directory
            else if (targetIsDir)
            {
                targetToList = FileNameHelpers.AppendDirTag(targetToList);
                asFiles = await ConnectionObject.FileSystemObject.GetFiles(targetToList);
                asDirectories = await ConnectionObject.FileSystemObject.GetDirectories(targetToList);
            }
            else
            {
                return GetMessage(550, string.Format("\"{0}\" not exists", sMessage));
            }

            // generate the response
            string sFileList = await BuildReply(asFiles, asDirectories);

            SocketHelpers.Send(ConnectionObject.Socket, string.Format("213-Begin STAT \"{0}\":\r\n", sMessage), ConnectionObject.Encoding);

            SocketHelpers.Send(ConnectionObject.Socket, sFileList, ConnectionObject.Encoding);

            return GetMessage(213, string.Format("{0} successful.", Command));
        }

        protected override async Task<string> BuildReply(string[] asFiles, string[] asDirectories)
        {
            return await BuildLongReply(asFiles, asDirectories);
        }
    }
}