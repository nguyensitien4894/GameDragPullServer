using NLog;
using System;
using System.Diagnostics;
using System.Text;

namespace TraditionGame.Utilities
{
    public class NLogManager
    {
        /// <summary>
        /// 
        /// </summary>
        volatile static Logger Log = LogManager.GetLogger("infoLog");
        volatile static Logger AuthenLog = LogManager.GetLogger("Authen");
        volatile static Logger LogSession = LogManager.GetLogger("LogSession");
        volatile static Logger momoLog = LogManager.GetLogger("Momo");
        volatile static Logger cockFightLog = LogManager.GetLogger("CockFight");

        public static void MomoLog(string message)
        {
            momoLog.Info(":\t" + GetCalleeString() + Environment.NewLine + "\t" + message);
        }

        public static void CockFightLog(string message)
        {
            cockFightLog.Info(":\t" + GetCalleeString() + Environment.NewLine + "\t" + message);
        }

        /// <summary>
        /// Writes an Error to the log.
        /// </summary>
        /// <param name="ex"></param>
        public static void PublishException(Exception ex)
        {
            Log.Error(string.Format("\t{0}{1}{2}{3}{4}", GetCalleeString(), Environment.NewLine, ex.Message, Environment.NewLine, ex.StackTrace));
        }

        public static void LogError(string message)
        {
            Log.Error(":\t" + GetCalleeString() + Environment.NewLine + "\t" + message);
        }

        /// <summary>
        /// Writes an Error to the log.
        /// </summary>
        /// <param name="ex"></param>
        public static void LogMessage(string message)
        {
            Log.Info(":\t" + GetCalleeString() + Environment.NewLine + "\t" + message);
        }

        public static void LogSessionError(string message)
        {
            LogSession.Info(":\t" + GetCalleeString() + Environment.NewLine + "\t" + message);
        }

        public static void WarningAuthen(string message)
        {
            AuthenLog.Warn(message);
        }

        public static void LogMessageAuthen(string message)
        {
            AuthenLog.Info(message);
        }

        public static void ExceptionAuthen(Exception ex)
        {
            AuthenLog.Error(ex);
        }

        /// <summary>
        /// Writes an Error to the log.
        /// </summary>
        /// <param name="ex"></param>
        public static void Warning(string message)
        {
            Log.Warn(string.Format(":\t{0}{1}\t{2}", GetCalleeString(), Environment.NewLine, message));
        }

        /// <summary>
        /// Writes an Debug to the log.
        /// </summary>
        /// <param name="ex"></param>
        public static void Debug(string message)
        {
            Log.Error(string.Format(":\t{0}{1}\t{2}", GetCalleeString(), Environment.NewLine, message));
        }

        public static string GetValueOfObject(object ob)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (System.Reflection.PropertyInfo piOrig in ob.GetType().GetProperties())
                {
                    object editedVal = ob.GetType().GetProperty(piOrig.Name).GetValue(ob, null);
                    sb.AppendFormat("{0}:{1}\t ", piOrig.Name, editedVal);
                }
            }
            catch
            {
            }
            return sb.ToString();
        }

        private static string GetCalleeString()
        {
            foreach (StackFrame sf in new StackTrace().GetFrames())
            {
                if (!string.IsNullOrEmpty(sf.GetMethod().ReflectedType.Namespace) && !typeof(NLogManager).FullName.StartsWith(sf.GetMethod().ReflectedType.Namespace))
                {
                    return string.Format("{0}.{1} ", sf.GetMethod().ReflectedType.Name, sf.GetMethod().Name);
                }
            }

            return string.Empty;
        }
    }
}