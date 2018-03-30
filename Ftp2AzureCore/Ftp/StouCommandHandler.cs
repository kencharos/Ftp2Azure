using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// STOU command handler, superfluous at this site
    /// Store unique
    /// </summary>
    internal class StouCommandHandler : FtpCommandHandler
    {
        public StouCommandHandler(FtpConnectionObject connectionObject)
            : base("STOU", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(202, "Use STOR instead");
        }
    }
}