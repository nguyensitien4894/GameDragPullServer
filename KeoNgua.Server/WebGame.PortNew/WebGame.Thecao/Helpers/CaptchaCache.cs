using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;

namespace MsWebGame.Thecao.Helpers
{
    public class CaptchaCache
    {
        private readonly static Lazy<CaptchaCache> _instance = new Lazy<CaptchaCache>(() => new CaptchaCache());

        public static CaptchaCache Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        //   private ConcurrentDictionary<string, CaptchaText> CaptchaTexts = new ConcurrentDictionary<string, CaptchaText>();
        private string randomkey = "&938383IEhHkelwew";

        public string[] GetCaptcha()
        {
           // RemoveOldCaptcha();
            CaptchaImage captcha = new CaptchaImage();
            string imageData = ImageGenarate(captcha);
            string verifyHash = randomkey+ captcha.Text.ToUpper();
            string key = Security.SHA256Encrypt(verifyHash);

            //CaptchaTexts.TryAdd(key, new CaptchaText()
            //{
            //    CreatedDate = DateTime.Now,
            //    Text = captcha.Text
            //});

            return new string[] { key, imageData };
        }

        public int VerifyCaptcha(string captchaText, string verify)
        {
            try
            {
                // RemoveOldCaptcha();
                //CaptchaText savedToken;
                //CaptchaTexts.TryGetValue(verify, out savedToken);

                //if (captchaText.ToUpper().Equals(savedToken.Text))
                //{
                //    //if verified then remove captcha
                //    CaptchaTexts.TryRemove(verify, out savedToken);
                //    return 1;
                //}
                //CaptchaTexts.TryRemove(verify, out savedToken);
                string key = Security.SHA256Encrypt(randomkey+captchaText.ToUpper());
                return key == verify ? 1 : -1;
                
            }
            catch (Exception exception)
            {
                NLogManager.PublishException(exception);
                return -99;
            }
        }

        //public void RemoveOldCaptcha()
        //{
        //    try
        //    {
        //        var lstOldCaptcha = CaptchaTexts.Where(x => x.Value != null && DateTime.Now.Subtract(x.Value.CreatedDate).TotalMinutes >= 10).ToList();
        //        foreach (var c in lstOldCaptcha)
        //        {
        //            CaptchaText cap;
        //            CaptchaTexts.TryRemove(c.Key, out cap);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //    }
        //}

        private string ImageGenarate(CaptchaImage captcha)
        {
            var codec = GetEncoderInfo("image/jpeg");
            // set image quality
            var eps = new EncoderParameters();
            eps.Param[0] = new EncoderParameter(Encoder.Quality, (long)95);
            var ms = new MemoryStream();
            captcha.RenderImage().Save(ms, codec, eps);

            byte[] bitmapBytes = ms.GetBuffer();
            string result = Convert.ToBase64String(bitmapBytes, Base64FormattingOptions.InsertLineBreaks);

            ms.Close();
            ms.Dispose();
            GC.Collect();
            return result;
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var myEncoders =
                ImageCodecInfo.GetImageEncoders();

            foreach (var myEncoder in myEncoders)
                if (myEncoder.MimeType == mimeType)
                    return myEncoder;
            return null;
        }
    }

    public class CaptchaText
    {
        public DateTime CreatedDate { get; set; }
        public string Text { get; set; }
    }
}