using System;

namespace DataAccess.Data
{
    public class TARGET_ORDER
    {
        public int Id { get; set; }

        public string BaseCur { get; set; }

        public string SwapCur { get; set; }

        public string OrderId { get; set; }

        /// <summary>
        /// 0：买单挂单中，2：卖单挂单中，1：完成
        /// </summary>
        public string Completed { get; set; }

        public double Price { get; set; }

        public double SwapAmount { get; set; }

        public string NeedSell { get; set; }

        public string SellOrderId { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return SwapCur + "/" + BaseCur + "\t " + Price + "\t" + Completed + "\t" + OrderId;
        }
    }
}