using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FcoinResponse<T>
    {
        public string status { get; set; }

        public T data { get; set; }
    }
}
