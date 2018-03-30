using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.FileSystem;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// RNFR command handler
    /// Starts a rename file operation
    /// </summary>
    internal class RenameStartCommandHandler : FtpCommandHandler
    {
        public RenameStartCommandHandler(FtpConnectionObject connectionObject)
            : base("RNFR", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            if (sMessage.Length == 0)
                return GetMessage(501, "Syntax error. RNFR needs a parameter");

            string sFile = GetPath(sMessage);

            // check whether file exists
            if (!await ConnectionObject.FileSystemObject.FileExists(sFile))
            {
                return GetMessage(550, string.Format("File {0} not exists. Rename directory not supported.", sMessage));
            }

            ConnectionObject.FileToRename = sFile;

            return GetMessage(350, string.Format("Rename file started ({0}).", sFile));
        }
    }
}