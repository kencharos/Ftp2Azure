using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// ALLO command hander
    /// allocate space for files, no need in this ftp server
    /// </summary>
    internal class AlloCommandHandler : FtpCommandHandler
    {
        public AlloCommandHandler(FtpConnectionObject connectionObject)
            : base("ALLO", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(202, "Do not require to declare the maximum size of the file beforehand");
        }
    }
}