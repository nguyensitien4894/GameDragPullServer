using System;
using System.Configuration;
using TraditionGame.Utilities.Security;

namespace TraditionGame.Utilities
{
    public sealed class Config
    {
        private static readonly Config _instance = new Config();

        private readonly string _bettingConn;
        private readonly string _bettingLogConn;
        private readonly string _BettingEventDBConn;
        private readonly string _emulatorConn;

        private readonly string _thuycungConn;
        private readonly string _thuycungLogConn;

        private readonly string _demthuonghaiConn;
        private readonly string _demthuonghaiLogConn;

        private readonly string _girljumpConn;
        private readonly string _girljumpLogConn;

        private readonly string _taydukyConn;
        private readonly string _taydukyLogConn;

        private readonly string _egyptConn;
        private readonly string _egyptLogConn;

        private readonly string _songokuConn;
        private readonly string _songokuLogConn;

        private readonly string _bumbumConn;
        private readonly string _bumbumLogConn;

        private readonly string _luckydiceConn;
        private readonly string _luckydiceLogConn;

        private readonly string _blockbusterConn;
        private readonly string _blockbusterLogConn;

        private readonly string _xpokerConn;
        private readonly string _xpokerLogConn;

        private readonly string _HorseHunterConn;
        private readonly string _HorseHunterLogConn;

        private readonly string _vqmmConn;
        private readonly string _vqmmLogConn;

        private readonly string _monkeyclimbConn;
        private readonly string _dragontigerConn;

        private readonly string _bettingCardConn;
        private readonly string _bettingBacayCardConn;
        private readonly string _bettingCardLogConn;
        private readonly string _sedieConn;

        private readonly string _gamesConn;
        private readonly string _gameLogsConn;
        private readonly string _xoSoConn;
        public static Config Instance
        {
            get { return _instance; }
        }

        Config()
        {
            bool encrypted = true;
            string encryptedStr = ConfigurationManager.AppSettings["Encrypted"];
            if (encryptedStr != null && encryptedStr != string.Empty)
            {
                encrypted = Boolean.Parse(encryptedStr);
            }
            _BettingEventDBConn = GetConnStr("BettingEventDBConn", encrypted);
            _bettingConn = GetConnStr("BettingConn", encrypted);
            _bettingLogConn = GetConnStr("BettingLogConn", encrypted);

            _emulatorConn = GetConnStr("EmulatorConn", encrypted);

            _thuycungConn = GetConnStr("ThuyCungConn", encrypted);
            _thuycungLogConn = GetConnStr("ThuyCungLogConn", encrypted);

            _demthuonghaiConn = GetConnStr("DemThuongHaiConn", encrypted);
            _demthuonghaiLogConn = GetConnStr("DemThuongHaiLogConn", encrypted);

            _girljumpConn = GetConnStr("GirlJumpConn", encrypted);
            _girljumpLogConn = GetConnStr("GirlJumpLogConn", encrypted);

            _taydukyConn = GetConnStr("TaydukyConn", encrypted);
            _taydukyLogConn = GetConnStr("TaydukyLogConn", encrypted);

            _egyptConn = GetConnStr("EgyptConn", encrypted);
            _egyptLogConn = GetConnStr("EgyptLogConn", encrypted);

            _songokuConn = GetConnStr("SongokuConn", encrypted);
            _songokuLogConn = GetConnStr("SongokuLogConn", encrypted);

            _bumbumConn = GetConnStr("BumBumConn", encrypted);
            _bumbumLogConn = GetConnStr("BumBumLogConn", encrypted);

            _luckydiceConn = GetConnStr("LuckyDiceConn", encrypted);
            _luckydiceLogConn = GetConnStr("LuckyDiceLogConn", encrypted);

            _blockbusterConn = GetConnStr("BlockBusterConn", encrypted);
            _blockbusterLogConn = GetConnStr("BlockBusterLogConn", encrypted);

            _xpokerConn = GetConnStr("XpokerConn", encrypted);
            _xpokerLogConn = GetConnStr("XpokerLogConn", encrypted);

            _HorseHunterConn = GetConnStr("HorseHunterConn", encrypted);
            _HorseHunterLogConn = GetConnStr("HorseHunterLogConn", encrypted);

            _vqmmConn = GetConnStr("VqmmConn", encrypted);
            _vqmmLogConn = GetConnStr("VqmmLogConn", encrypted);

            _monkeyclimbConn = GetConnStr("MonkeyClimbConn", encrypted);
            _dragontigerConn = GetConnStr("DragonTigerConn", encrypted);

            _bettingCardConn = GetConnStr("BettingCardConn", encrypted);
            _bettingBacayCardConn =GetConnStr("BettingBacayCardConn", encrypted);
            _bettingCardLogConn = GetConnStr("BettingCardLogConn", encrypted);
            _sedieConn = GetConnStr("SedieConn", encrypted);

            _gamesConn = GetConnStr("GamesConn", encrypted);
            _gameLogsConn = GetConnStr("GameLogsConn", encrypted);

            _xoSoConn = GetConnStr("XoSoConn", encrypted);
        }

        public static string BettingConn { get { return _instance._bettingConn; } }
        public static string BettingLogConn { get { return _instance._bettingLogConn; } }
        public static string BettingEventDBConn { get { return _instance._BettingEventDBConn; } }
        
        public static string EmulatorConn { get { return _instance._emulatorConn; } }

        public static string ThuyCungConn { get { return _instance._thuycungConn; } }
        public static string ThuyCungLogConn { get { return _instance._thuycungLogConn; } }
        public static string DemThuongHaiConn { get { return _instance._demthuonghaiConn; } }
        public static string DemThuongHaiLogConn { get { return _instance._demthuonghaiLogConn; } }

        public static string GirlJumpConn { get { return _instance._girljumpConn; } }
        public static string GirlJumpLogConn { get { return _instance._girljumpLogConn; } }

        public static string TaydukyConn { get { return _instance._taydukyConn; } }
        public static string TaydukyLogConn { get { return _instance._taydukyLogConn; } }

        public static string EgyptConn { get { return _instance._egyptConn; } }
        public static string EgyptLogConn { get { return _instance._egyptLogConn; } }

        public static string SonGoKuConn { get { return _instance._songokuConn; } }
        public static string SonGoKuLogConn { get { return _instance._songokuLogConn; } }

        public static string BumBumConn { get { return _instance._bumbumConn; } }
        public static string BumBumLogConn { get { return _instance._bumbumLogConn; } }

        public static string LuckyDiceConn { get { return _instance._luckydiceConn; } }
        public static string LuckyDiceLogConn { get { return _instance._luckydiceLogConn; } }

        public static string BlockBusterConn { get { return _instance._blockbusterConn; } }
        public static string BlockBusterLogConn { get { return _instance._blockbusterLogConn; } }

        public static string XpokerConn { get { return _instance._xpokerConn; } }
        public static string XpokerLogConn { get { return _instance._xpokerLogConn; } }

        public static string HorseHunterConn { get { return _instance._HorseHunterConn; } }
        public static string HorseHunterLogConn { get { return _instance._HorseHunterLogConn; } }

        public static string VqmmConn { get { return _instance._vqmmConn; } }
        public static string VqmmLogConn { get { return _instance._vqmmLogConn; } }

        public static string MonkeyClimbConn { get { return _instance._monkeyclimbConn; } }
        public static string DragonTigerConn { get { return _instance._dragontigerConn; } }

        public static string BettingCardConn { get { return _instance._bettingCardConn; } }
        public static string BettingBacayCardConn { get { return _instance._bettingBacayCardConn; } }
        public static string BettingCardLogConn { get { return _instance._bettingCardLogConn; } }
        public static string SedieConn { get { return _instance._sedieConn; } }

        public static string GamesConn { get { return _instance._gamesConn; } }
        public static string GameLogsConn { get { return _instance._gameLogsConn; } }
        public static string XoSoConn { get { return _instance._xoSoConn; } }
        

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
                RijndaelEnhanced re = new RijndaelEnhanced("redblack", "@1B2c3D4e5F6g7H8");
               
                return connStr == "" ? "" : re.Decrypt(connStr);
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

        public const int MAX_BOTID = 100000;
    }
}
