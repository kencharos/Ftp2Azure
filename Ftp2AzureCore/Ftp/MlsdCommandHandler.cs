using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.General;
using Ftp2Azure.General;
using System.Text;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// MLSD command handler
    /// only list content under directories
    /// </summary>
    class MlsdCommandHandler : MlsxCommandHandlerBase
    {
        public MlsdCommandHandler(FtpConnectionObject connectionObject)
            : base("MLSD", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            // Get the dir to list
            string targetToList = GetPath(sMessage);

            // checks the dir name
            if (!FileNameHelpers.IsValid(targetToList))
            {
                return GetMessage(501, string.Format("\"{0}\" is not a valid directory name", sMessage));
            }

            // specify the directory tag
            targetToList = FileNameHelpers.AppendDirTag(targetToList);

            bool targetIsDir = await ConnectionObject.FileSystemObject.DirectoryExists(targetToList);

            if (!targetIsDir)
                return GetMessage(550, string.Format("Directory \"{0}\" not exists", targetToList));

            #region Generate response

            StringBuilder response = new StringBuilder();

            string[] files = await ConnectionObject.FileSystemObject.GetFiles(targetToList);
            string[] directories = await ConnectionObject.FileSystemObject.GetDirectories(targetToList);

            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileInfo = await ConnectionObject.FileSystemObject.GetFileInfo(file);

                    response.Append(GenerateEntry(fileInfo));

                    response.Append("\r\n");
                }
            }

            if (directories != null)
            {
                foreach (var dir in directories)
                {
                    var dirInfo = await ConnectionObject.FileSystemObject.GetDirectoryInfo(dir);

                    response.Append(GenerateEntry(dirInfo));

                    response.Append("\r\n");
                }
            }

            #endregion

            #region Write response

            var socketData = new FtpDataSocket(ConnectionObject);

            if (!socketData.Loaded)
            {
                return GetMessage(425, "Unable to establish the data connection");
            }

            SocketHelpers.Send(ConnectionObject.Socket, "150 Opening data connection for MLSD\r\n", ConnectionObject.Encoding);

            // ToDo, send response according to ConnectionObject.DataType, i.e., Ascii or Binary
            socketData.Send(response.ToString(), Encoding.UTF8);
            socketData.Close();

            #endregion

            return GetMessage(226, "MLSD successful");
        }
    }
}
