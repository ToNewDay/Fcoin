namespace DataAccess.Param
{
    public class MakeOrderParam
    {
        public string symbol { get; set; } /*无   交易对*/
        public string side { get; set; } /*无 交易方向*/
        private string _type = "limit";
        public string type { get { return _type; } set { _type = value; } } /* 无   订单类型*/
        public string price { get; set; } /*            无 价格*/
        public string amount { get; set; } /*   无   下单量*/

        private string _exchange = "main";
        public string exchange { get { return _exchange; } set { _exchange = value; } } /*     无 交易区*/

        private string _account_type = "spot";
        public string account_type { get { return _account_type; } set { _account_type = value; } } /*     无   账户类型(币币交易不需要填写，杠杆交易：margin)*/
    }
}