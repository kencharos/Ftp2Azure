using System;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

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
                return RoleEnvironment.GetConfigurationSettingValue("FtpAccount");
            }
        }

        public static Modes Mode
        {
            get
            {
                return (Modes)Enum.Parse(typeof(Provider.Modes), RoleEnvironment.GetConfigurationSettingValue("Mode"));
            }
        }

        public static bool QueueNotification
        {
            get
            {
                return bool.Parse(RoleEnvironment.GetConfigurationSettingValue("QueueNotification"));
            }
        }

        public static int MaxIdleSeconds
        {
            get
            {
                return int.Parse(RoleEnvironment.GetConfigurationSettingValue("MaxIdleSeconds"));
            }
        }

        public static string FtpServerHost
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("FtpServerHost");
            }
        }
    }
}