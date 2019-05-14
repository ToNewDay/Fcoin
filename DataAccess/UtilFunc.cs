using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataAccess
{
    public static class UtilFunc
    {
        public static Symbol FormtStr(string symbolStr)
        {
            List<string> BaseCurrList = new List<string>()
            {
                "USDT","PAX","BTC","ETH"
            };
            foreach (string baseCurr in BaseCurrList)
            {
                if (symbolStr.ToUpper().Contains(baseCurr))
                {
                    return new Symbol()
                    {
                        BaseCurr = baseCurr,
                        SwapCurr = symbolStr.ToUpper().Replace(baseCurr, "")
                    };
                };
            }
            return null;
        }
    }
}
