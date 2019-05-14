namespace DataAccess.Param
{
    public class OrderSearchParam
    {
        //        symbol 交易对
        //states 订单状态，多种状态联合查询：submitted,partial_filled,partial_canceled,filled,canceled,中间用逗号隔开
        //before      查询某个时间戳之前的订单
        //after       查询某个时间戳之后的订单
        //limit       每页的订单数量，默认为 20 条，最大100
        //account_type        杠杆：margin

        public string symbol { get; set; }
        private string _states = "submitted,partial_filled,partial_canceled,filled,canceled";
        public string states { get { return _states; } set { _states = value; } }
        public string before { get; set; }
        public string after { get; set; }
        public string limit { get; set; }
        public string account_type { get; set; }
    }
}