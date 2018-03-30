using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// NLST command handler
    /// name list
    /// </summary>
    internal class NlstCommandHandler : ListCommandHandlerBase
    {
        public NlstCommandHandler(FtpConnectionObject connectionObject)
            : base("NLST", connectionObject)
        {
        }

        protected override Task<string> BuildReply(string[] asFiles, string[] asDirectories)
        {
            return Task.FromResult<string>(BuildShortReply(asFiles, asDirectories));
        }
    }
}