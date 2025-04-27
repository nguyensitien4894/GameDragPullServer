using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Utils
{
    public class MessageMuaThe24h
    {
        private static readonly Lazy<MessageMuaThe24h> _instance = new Lazy<MessageMuaThe24h>(() => new MessageMuaThe24h());

        private readonly Dictionary<int, string> Data_Array = new Dictionary<int, string>();

        public static MessageMuaThe24h Instance
        {
            get { return _instance.Value; }
        }

        private MessageMuaThe24h()
        {
            Data_Array.Add(0, "Mua thẻ 24h thành công!");
            Data_Array.Add(5, "Kho hết thẻ!");
            Data_Array.Add(278, "Không tồn tại tài khoản!");
            Data_Array.Add(777, "Lỗi hệ thông!");
            Data_Array.Add(778, "Không đủ số dư!");
        }

        public string GetMessage(int key)
        {
            string msg = string.Empty;
            Data_Array.TryGetValue(key, out msg);
            return msg;
        }
    }
}