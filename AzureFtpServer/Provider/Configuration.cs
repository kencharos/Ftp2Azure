using System;
using System.Configuration;

namespace AzureFtpServer.Provider
{
    public enum Modes
    {
        Live,
        Debug
    }

    public class StorageProviderConfiguration
    {
        
        public static string FtpAccount
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpAccount"];
            }
        }

        public static Modes Mode
        {
            get
            {
                return (Modes)Enum.Parse(typeof(Provider.Modes), ConfigurationManager.AppSettings["Mode"]);
            }
        }

        public static bool QueueNotification
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["QueueNotification"]);
            }
        }

        public static int MaxIdleSeconds
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["MaxIdleSeconds"]);
            }
        }

        public static string FtpServerHost
        {
            get
            {
                return ConfigurationManager.AppSettings["FtpServerHost"];
            }
        }
    }
}