using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;

namespace Ftp2Azure.Provider
{
    public enum Modes
    {
        Live,
        Debug
    }

    // TODO singleton instance;
    public static class StorageProviderConfiguration
    {
        static IConfiguration _conf;


        public static void Init(IConfiguration conf)
        {
            _conf = conf;
        }


        public static string FtpAccount
        {
            get
            {
                return _conf["FtpAccount"];
            }
        }

        public static Modes Mode
        {
            get
            {
                return (Modes)Enum.Parse(typeof(Provider.Modes), _conf["Mode"]);
            }
        }

        public static bool QueueNotification
        {
            get
            {
                return bool.Parse(_conf["QueueNotification"]);
            }
        }

        public static int MaxIdleSeconds
        {
            get
            {
                return int.Parse(_conf["MaxIdleSeconds"]);
            }
        }

        public static string FtpServerHost
        {
            get
            {
                return _conf["FtpServerHost"];
            }
        }
    }
}