using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsTraditionGame.Utilities.Security;

namespace MsTraditionGame.Utilities.Config
{
    public sealed class Config
    {
        private static readonly Config _instance = new Config();

        private readonly string _GateConnectionString;
        private readonly string _TqxConnectionString;
        private readonly string _CdConnectionString;
        private readonly string _HistoryConnectionString;
        //private readonly string _ChoSaoConnectionString;
        private readonly string _LuckySpinConnectionString;
        private readonly string _EmulatorConnectionString;
        private readonly string _LuckySpinHistoryConnectionString;
        private readonly string _LuckyDiceHistoryConnectionString;
        private readonly string _LuckyDiceConnectionString;
        private readonly string _ASPSTATEConnectionString; 
        public static Config Instance
        {
            get
            {
                return _instance;
            }
        }

        Config()
        {
            _GateConnectionString = GetConnStr("GateConnectionString");
            _TqxConnectionString = GetConnStr("TQXConnectionString");
            _CdConnectionString = GetConnStr("CDConnectionString");
            _HistoryConnectionString = GetConnStr("BettingCoreHistory_ConnectionString");
            //_ChoSaoConnectionString = GetConnStr("ChoSao_ConnectionString");
            _LuckySpinConnectionString = GetConnStr("LuckySpin_ConnectionString");
            _EmulatorConnectionString = GetConnStr("EmulatorConnectionString");
            _LuckySpinHistoryConnectionString = GetConnStr("LuckySpinHistory_ConnectionString");
            _LuckyDiceHistoryConnectionString = GetConnStr("LuckyDiceHistory_ConnectionString");
            _LuckyDiceConnectionString = GetConnStr("LuckyDice_ConnectionString");
            _ASPSTATEConnectionString = GetConnStr("ASPSTATE_ConnectionString");
        }

        public static string GateConnectionString
        {
            get { return _instance._GateConnectionString; }
        }
        //public static string ChoSaoConnectionString
        //{
        //    get { return _instance._ChoSaoConnectionString; }
        //}
        public static string ASPSTATEConnectionString
        {
            get { return _instance._ASPSTATEConnectionString; }
        }

        public static string TQXConnectionString
        {
            get { return _instance._TqxConnectionString; }
        }

        public static string CDConnectionString
        {
            get { return _instance._CdConnectionString; }
        }

        public static string HistoryConnectionString
        {
            get { return _instance._HistoryConnectionString; }
        }

        public static string LuckySpinConnectionString
        {
            get { return _instance._LuckySpinConnectionString; }
        }

        public static string EmulatorConnectionString
        {
            get { return _instance._EmulatorConnectionString; }
        }

        public static string LuckySpinHistoryConnectionString
        {
            get { return _instance._LuckySpinHistoryConnectionString; }
        }

        public static string LuckyDiceHistoryConnectionString
        {
            get { return _instance._LuckyDiceHistoryConnectionString; }
        }

        public static string LuckyDiceConnectionString
        {
            get { return _instance._LuckyDiceConnectionString; }
        }

        public static string GetConnStr(string name)
        {
            return GetConnStr(name, true);
        }

        public static string GetConnStr(string name, bool encrypted)
        {
            string connStr = ConfigurationManager.ConnectionStrings[name] == null ? "" : ConfigurationManager.ConnectionStrings[name].ConnectionString;

            if (!encrypted)
            {
                return connStr;
            }

            try
            {
                return connStr == "" ? "" : new RijndaelEnhanced("redblack", "@1B2c3D4e5F6g7H8").Decrypt(connStr);
            }
            catch
            {
                return connStr;
            }
        }

        public static void UpdateAppSettings(AppSettingsSection appSettings, string key, string value)
        {
            if (appSettings.Settings[key] == null)
            {
                return;
            }

            appSettings.Settings[key].Value = value;
        }

        public static void UpdateConnectionStrings(ConnectionStringsSection connectStrings, string name, bool encrypt)
        {
            if (connectStrings.ConnectionStrings[name] == null)
            {
                return;
            }

            string connectionString = connectStrings.ConnectionStrings[name].ConnectionString;
            if (encrypt)
            {
                connectionString = new RijndaelEnhanced("redblack", "@1B2c3D4e5F6g7H8").Encrypt(connectionString);
            }

            connectStrings.ConnectionStrings[name].ConnectionString = connectionString;
        }

        public static string GetAppSettings(string key, string defaultValue = "")
        {
            string value = defaultValue;

            if (string.IsNullOrEmpty(key))
                return value;

            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch { }

            return value;
        }

        public static string GetAppSettings(string key, bool encrypted, string defaultValue = "")
        {
            string value = defaultValue;
            if (string.IsNullOrEmpty(key))
                return value;

            value = ConfigurationManager.AppSettings[key] == null ? "" : ConfigurationManager.AppSettings[key];

            if (!encrypted)
            {
                return value;
            }

            try
            {
                return value == "" ? "" : new RijndaelEnhanced("redblack", "@1B2c3D4e5F6g7H8").Decrypt(value);
            }
            catch
            {
                return value;
            }
        }

        public static int GetIntegerAppSettings(string key, int defaultValue = 0)
        {
            int value = defaultValue;

            if (string.IsNullOrEmpty(key))
                return value;

            try
            {
                value = Int32.Parse(ConfigurationManager.AppSettings[key]);
            }
            catch { }

            return value;
        }
    }
}
