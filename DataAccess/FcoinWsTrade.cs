using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FcoinWsTrade
    {
        public double amount { get; set; }
        public long id { get; set; }
        public double price { get; set; }
        public string side { get; set; }
        public double ts { get; set; }
        public string type { get; set; }
    }

}
