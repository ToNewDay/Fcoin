namespace DataAccess
{
    public class OrderInfo
    {
        public string amount { get; set; }
        public long created_at { get; set; }
        public string exchange { get; set; }
        public string executed_value { get; set; }
        public string fees_income { get; set; }
        public string fill_fees { get; set; }
        public string filled_amount { get; set; }
        public string id { get; set; }
        public string price { get; set; }
        public string side { get; set; }
        public string source { get; set; }
        public string state { get; set; }
        public string symbol { get; set; }
        public string type { get; set; }
    }
}