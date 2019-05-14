using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataAccess
{
    public class TRADE_INFO_PER_MIN
    {
        public TRADE_INFO_PER_MIN()
        {

        }
        public TRADE_INFO_PER_MIN(List<TRADE_DATA> param)
        {
            if (param.Count == 0)
            {
                return;
            }

            this.Gmt8DataTime = param.First().CreateTime.ToString("yyyyMMdd HH:mm");

            this.Symbol = param.First().Symbol;

            Symbol symbol = UtilFunc.FormtStr(this.Symbol);
            if (symbol != null)
            {
                this.BaseCur = symbol.BaseCurr;
                this.SwapCur = symbol.SwapCurr;
            }

            this.AvgPrice = param.Sum(m => m.Price * m.Amount) / param.Sum(m => m.Amount);

            this.TotalTradeBase = param.Sum(m => m.Price * m.Amount);

            this.MaxPrice = param.Max(m => m.Price);

            this.MinPrice = param.Min(m => m.Price);


        }
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string SwapCur { get; set; }

        public string BaseCur { get; set; }

        public double AvgPrice { get; set; }

        public double MaxPrice { get; set; }

        public double MinPrice { get; set; }

        public string Gmt8DataTime { get; set; }

        public double TotalTradeBase { get; set; }
    }
}
