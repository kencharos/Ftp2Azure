using Ftp2Azure.Azure;
using Ftp2Azure.Ftp;
using Ftp2Azure.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ftp2Azure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var _server = new FtpServer(new AzureFileSystemFactory());

            _server.NewConnection += nId =>
                Console.WriteLine("Connection: {0} accepted", nId);

            // This is a sample worker implementation. Replace with your logic.
            Console.WriteLine("Information: FTPRole entry point called");

            while (true)
            {
                if (_server.Started)
                {
                    Thread.Sleep(10000);
                    //Console.WriteLine("Information: Server is alive");
                }
                else
                {
                    _server.Start();
                    Console.WriteLine("Control: Server starting");
                }
            }
        }
    }
}
