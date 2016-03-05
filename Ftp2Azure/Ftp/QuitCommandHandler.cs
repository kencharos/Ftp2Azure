using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// QUIT command handler
    /// </summary>
    internal class QuitCommandHandler : FtpCommandHandler
    {
        public QuitCommandHandler(FtpConnectionObject connectionObject)
            : base("QUIT", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(220, "Goodbye");
        }
    }
}