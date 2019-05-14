using DataAccess;
using DataAccess.Param;
using NETCore.Encrypt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

namespace FcoinUtil
{
    public class FcoinClient
    {
        private string key = "3600d0a74aa3410fb3b1996cca2419c8";

        private string secret = "3600d0a74aa3410fb3b1996cca2419c8";

        public FcoinClient(string key, string secret)
        {
            this.key = key;
            this.secret = secret;
        }

        /// <summary>
        /// 获取账户资产
        /// </summary>
        /// <returns></returns>
        public FcoinResponse<List<BalanceInfo>> GetBalanceList()
        {
            FcoinResponse<List<BalanceInfo>> balanceInfo = new FcoinResponse<List<BalanceInfo>>();
            try
            {
                string result = SendData("https://api.fcoin.com/v2/accounts/balance", new Dictionary<string, string>(), "GET");
                balanceInfo = JsonConvert.DeserializeObject<FcoinResponse<List<BalanceInfo>>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("get balance error " + ex.ToString());
            }
            return balanceInfo;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public FcoinResponse<List<OrderInfo>> GetOrderList(OrderSearchParam param)
        {
            FcoinResponse<List<OrderInfo>> orderInfo = new FcoinResponse<List<OrderInfo>>();
            try
            {
                string result = SendDataWithObject("https://api.fcoin.com/v2/orders", param, "GET");
                orderInfo = JsonConvert.DeserializeObject<FcoinResponse<List<OrderInfo>>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("get order list error " + ex.ToString());
            }
            return orderInfo;
        }

        /// <summary>
        /// 查询订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public FcoinResponse<OrderInfo> GetOrderDetail(string orderId)
        {
            FcoinResponse<OrderInfo> orderInfo = new FcoinResponse<OrderInfo>();
            try
            {
                string result = SendDataWithObject("https://api.fcoin.com/v2/orders/"+orderId, null, "GET");
                orderInfo = JsonConvert.DeserializeObject<FcoinResponse<OrderInfo>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("get order list error " + ex.ToString());
            }
            return orderInfo;
        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public FcoinResponse<string> MakerOrder(MakeOrderParam param)
        {
            param.amount = Math.Round(double.Parse(param.amount), 4).ToString();
            param.price = Math.Round(double.Parse(param.price),1).ToString();
            Console.WriteLine("maker order " + JsonConvert.SerializeObject(param));
            FcoinResponse<string> result = new FcoinResponse<string>();
            try
            {
                string resultResp = SendDataWithObject("https://api.fcoin.com/v2/orders" , param, "POST");
                result = JsonConvert.DeserializeObject<FcoinResponse<string>>(resultResp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("make order error " + ex.ToString());
            }
            return result;

        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public FcoinResponse<string> CancleOrder(string orderId)
        {
            FcoinResponse<string> result = new FcoinResponse<string>();
            try
            {
                string resultResp = SendDataWithObject("https://api.fcoin.com/v2/orders/"+ orderId + "/submit-cancel", null, "POST");
                result = JsonConvert.DeserializeObject<FcoinResponse<string>>(resultResp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("get order list error " + ex.ToString());
            }
            return result;
        }


        public string SendDataWithObject(string url,object data,string type)
        {
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            if (data == null)
            {
                return SendData(url, dicData, type);
            }
            Type dataType = data.GetType();
            foreach(var t in dataType.GetProperties())
            {
                dicData.Add(t.Name, t.GetValue(data)==null?"":t.GetValue(data).ToString());
            }

            return SendData(url, dicData, type);
        }

        public string SendData(string url, Dictionary<string, string> getData, string type)
        {
            HttpClient client = new HttpClient();
            List<string> siginData = getData == null ? new List<string>() : getData.ToList().OrderBy(m => m.Key).ToList().Select(m => m.Key + "=" + m.Value).ToList();
            string timeSpan = Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            string siginStr = string.Empty;
            if (type.ToUpper() == "GET")
            {
                siginStr = type.ToUpper() + url +(siginData.Count==0?"":("?"+ string.Join("&", siginData))) + timeSpan;

            }
            else
            {
                siginStr = type.ToUpper() + url + timeSpan  + string.Join("&", siginData);
            }
            siginStr = EncryptProvider.Base64Encrypt(siginStr);
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(secret);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(siginStr);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            siginStr = Convert.ToBase64String(hashBytes);
            client.DefaultRequestHeaders.Add("FC-ACCESS-KEY", key);
            client.DefaultRequestHeaders.Add("FC-ACCESS-SIGNATURE", siginStr);
            client.DefaultRequestHeaders.Add("FC-ACCESS-TIMESTAMP", timeSpan);
            //防止被墙
            url = url.Replace("fcoin", "ifukang");
            if (type == "GET")
            {
                return client.GetAsync(url + (siginData.Count == 0 ? "" : "?") + string.Join("&", siginData)).Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Dictionary<string, string> postData = new Dictionary<string, string>();
                if (getData != null)
                {
                    getData.ToList().OrderBy(m => m.Key).ToList().ForEach(m =>
                    {
                        postData.Add(m.Key, m.Value);
                    });
                }

                return client.PostAsJsonAsync(url, postData).Result.Content.ReadAsStringAsync().Result;
            }
        }






    }
}