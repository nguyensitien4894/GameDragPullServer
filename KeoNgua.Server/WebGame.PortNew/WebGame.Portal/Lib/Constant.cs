using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Lib
{
    public class Constant
    {
        //Charge config
        //bank
        public static string BANK_APPROVED = ConfigurationManager.AppSettings["BANK_APPROVED"];
        public static string CASHOUT_BANK_APPROVED = ConfigurationManager.AppSettings["CASHOUT_BANK_APPROVED"];

        //momo
        public static string MOMO_APPROVED = ConfigurationManager.AppSettings["MOMO_APPROVED"];
        public static string CASHOUT_MOMO_APPROVED = ConfigurationManager.AppSettings["CASHOUT_MOMO_APPROVED"];

        //viettelpay
        public static string VIETTELPAY_APPROVED = ConfigurationManager.AppSettings["VIETTELPAY_APPROVED"];
        public static string CASHOUT_VIETTELPAY_APPROVED = ConfigurationManager.AppSettings["CASHOUT_VIETTELPAY_APPROVED"];

        //zalopay
        public static string ZALO_APPROVED = ConfigurationManager.AppSettings["ZALO_APPROVED"];
        public static string CASHOUT_ZALO_APPROVED = ConfigurationManager.AppSettings["CASHOUT_ZALO_APPROVED"];

        //card 
        public static string CARD_APPROVED = ConfigurationManager.AppSettings["CARD_APPROVED"];
        public static string CASHOUT_CARD_APPROVED = ConfigurationManager.AppSettings["CASHOUT_CARD_APPROVED"];

        //QIEN GATEWAY
        public static string QUIEN_API_KEY = ConfigurationManager.AppSettings["QUIEN_API_KEY"];
        public static string QUIEN_PRIVATE_KEY = ConfigurationManager.AppSettings["QUIEN_PRIVATE_KEY"];
        public static string QUIEN_BANK_CALLBACK_URL = ConfigurationManager.AppSettings["QUIEN_BANK_CALLBACK_URL"];
        public static string QUIEN_MOMO_CALLBACK_URL = ConfigurationManager.AppSettings["QUIEN_MOMO_CALLBACK_URL"];
        public static string QUIEN_VIETTEL_PAY_CALLBACK_URL = ConfigurationManager.AppSettings["QUIEN_VIETTEL_PAY_CALLBACK_URL"];
        public static string QUIEN_ZALO_CALLBACK_URL = ConfigurationManager.AppSettings["QUIEN_ZALO_CALLBACK_URL"];
        public static string QUIEN_CARD_CALLBACK_URL = ConfigurationManager.AppSettings["QUIEN_CARD_CALLBACK_URL"];
        

        //Telegram 
        public static string CASHOUT_TELEGRAM_GROUP_CHAT_ID = ConfigurationManager.AppSettings["CASHOUT_TELEGRAM_GROUP_CHAT_ID"];



        public static string RUN_PORTAL_HANDLER = ConfigurationManager.AppSettings["RUN_PORTAL_HANDLER"];
        public enum CHARGETYPE : int
        {
            BANK = 1,
            MOMO,
            VIETTEL_PAY,
            ZALO,
            CARD
        }

        public enum CHARGE_CODE_STATUS : int
        {
            Processing = 1,
            Expired,
            Fail,
            Success
        }
    }
}