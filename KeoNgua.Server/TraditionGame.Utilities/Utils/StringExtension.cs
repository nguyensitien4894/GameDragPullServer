using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Utils
{
   public static   class   StringExtension
    {
      
            public static string TrimIfNotNull(this string value)
            {
                if (value != null)
                {
                    return value.Trim();
                }
                return null;
            }
        }
    
}
