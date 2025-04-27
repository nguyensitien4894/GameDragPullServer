using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTraditionGame.Utilities.Utils
{
    public class DateUtil
    {
        public static string TimeSpanToString(TimeSpan time)
        {
            if (time == null) return String.Empty;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds); ;
        }

    }
}
