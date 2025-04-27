using _1Pay;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebGame.Header.Utils._1Pays.SMSs
{
    public class SMSRequest
    {
        public static  string GenrateOTP(int lenthofpass)
        {
           // int lenthofpass = 7;
            string allowedChars = "";
            //allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            //allowedChars = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars = "1,2,3,4,5,6,7,8,9";
            char[] sep =
            {
            ','
            };
            string[] arr = allowedChars.Split(sep);
            string passwordString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < lenthofpass; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }
            return passwordString;
        }
        public static bool smsChargingRequest(string access_key, string command, string mo_message, string msisdn, string request_id, string request_time, string short_code, string signature)
        {
            //create signature true with parameters
            string secretKey = ConfigurationManager.AppSettings["SECRET_KEY"].ToString(); //require your secret key from 1pay
            My1Pay my1Pay = new My1Pay();
            string signatureTrue = my1Pay.generateSignature_Sms(access_key, command, mo_message, msisdn, request_id, request_time,
                       short_code, secretKey);

            //  string json = "";
            var serializer = new JavaScriptSerializer();

            //Security check at the merchant, not mandatory
            if (signature == signatureTrue)
            {
                //if sms content, amount, ... are ok. Return success, example:
                return true;
                // json = serializer.Serialize(new { status = 1, sms = "Giao dich thanh cong ... Hotline ...", type = "text" });
            }
            else
            {
                //if not. Return fail, ex:
                return false;
                // json = serializer.Serialize(new { status = 0, sms = "Giao dich khong thanh cong. Tin nhan se duoc hoan cuoc sau 20 ngay. Hotline...", type = "text" });
            }
            // return json;


        }

    }
}