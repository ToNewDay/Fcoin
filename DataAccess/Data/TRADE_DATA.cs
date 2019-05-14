using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{





   public class TRADE_DATA
    {
        public TRADE_DATA()
        {

        }
        public int Id { get; set; }

        public double Price { get; set; }

        public double Amount { get; set; }

        public string Symbol { get; set; }


        public string SwapCur { get; set; }

        public string BaseCur { get; set; }

        public string Side { get; set; }

        public long TradeId { get; set; }

        public DateTime CreateTime { get; set; }


        public TRADE_DATA(FcoinWsTrade param)
        {
            this.Amount = param.amount;
            this.Price = param.price;
            this.Symbol = param.type.Replace("trade.","");
            Symbol symbol= UtilFunc.FormtStr(this.Symbol);
            if (symbol != null)
            {
                this.BaseCur = symbol.BaseCurr;
                this.SwapCur = symbol.SwapCurr;
            }

            this.Side = param.side;
            this.TradeId = param.id;
            this.CreateTime = DateTime.UtcNow+TimeSpan.FromHours(8);
        }
    }
}
