#region old version

//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace fcoin
//{
//    internal class Program
//    {
//        private static void Main(string[] args)
//        {
//            GetWebSocket.connect_socket().Wait();
//        }
//    }

//    public static class GetWebSocket
//    {
//        private static ClientWebSocket web_socket;
//        public static bool connected = false;

//        public static async Task<bool> connect_socket()
//        {
//            try
//            {
//                web_socket = new ClientWebSocket();
//                await web_socket.ConnectAsync(new Uri("wss://exchange.fcoin.com/api/web/ws"), CancellationToken.None);
//                Dictionary<string, object> initmsg = new Dictionary<string, object>()
//                {
//                    {"args",new  List<string>(){ "all-tickers" } },
//                    {"cmd","sub" },
//                    {"id","tickers" }
//                };

//                string initMsgStr = JsonConvert.SerializeObject(initmsg);
//                ArraySegment<byte> sendbyte = new ArraySegment<byte>(Encoding.UTF8.GetBytes(initMsgStr));
//                DateTime startTime = DateTime.Now;
//                await web_socket.SendAsync(sendbyte, WebSocketMessageType.Text, true, CancellationToken.None);
//                while (true)
//                {
//                    ArraySegment<byte> byte_received = new ArraySegment<byte>(new byte[1024]);
//                    WebSocketReceiveResult json_notify_result = await web_socket.ReceiveAsync(byte_received, CancellationToken.None);
//                    string string_notify = Encoding.UTF8.GetString(byte_received.Array);
//                    Console.WriteLine(string_notify);
//                    if (DateTime.Now - startTime >= TimeSpan.FromSeconds(20))
//                    {
//                        Dictionary<string, object> pingmsg = new Dictionary<string, object>()
//                        {
//                            {"args",new  List<string>(){ DateTime.Now.Ticks.ToString() } },
//                            {"cmd","ping" },
//                        };
//                        string pingmsgStr = JsonConvert.SerializeObject(pingmsg);
//                        Console.weri
//                        ArraySegment<byte> sendPingbyte = new ArraySegment<byte>(Encoding.UTF8.GetBytes(pingmsgStr));
//                        await web_socket.SendAsync(sendbyte, WebSocketMessageType.Text, true, CancellationToken.None);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return connected;
//        }
//    }
//}

#endregion old version

using DataAccess;
using DataAccess.Data;
using FcoinUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fcoin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GetWebSocket getWebSocket = new GetWebSocket();
            getWebSocket.connect_socket().Wait();
        }
    }

    public class GetWebSocket
    {
        public async Task<bool> connect_socket()
        {
            bool connected = true;
            try
            {
                List<SYMBOL_DATA> symbolList = new List<SYMBOL_DATA>();
                using (AppDbContext db = new AppDbContext())
                {
                    symbolList = db.SYMBOL_DATA.OrderBy(m => m.Id).Take(5).ToList();
                }
                List<REWARD_DATA> rewareList = new List<REWARD_DATA>();
                List<Task> taskList = new List<Task>();
                using (AppDbContext db = new AppDbContext())
                {
                    rewareList = db.REWARD_DATA.ToList();
                }
                foreach (REWARD_DATA targetData in rewareList)
                {
                    Console.WriteLine("begin to get " + targetData.SwapCur + "/" + targetData.BaseCur + " data");
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>()
                        {
                                new Dictionary<string, object>()
                                {
                                    { "args",new List<string>{ "trade."+targetData.SwapCur.ToLower()+targetData.BaseCur.ToLower() } },
                                    { "cmd","sub" }
                                }
                        };
                    Task task = Task.Run(() =>
                    {
                        SaveTradeData(data);
                    });
                    taskList.Add(task);
                }
                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return connected;
        }

        public void SaveTradeData(List<Dictionary<string, object>> data)
        {
            using (AppDbContext db = new AppDbContext())
            {
                List<TRADE_DATA> dataList = new List<TRADE_DATA>();
                int minVal = DateTime.UtcNow.Minute;
                WebSocketUtil socket = new WebSocketUtil();
                socket.connect("wss://exchange.ifukang.com/api/web/ws", (string res) =>
                {
                    FcoinWsTrade resData = null;
                    try
                    {
                        resData = JsonConvert.DeserializeObject<FcoinWsTrade>(res);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (resData != null && resData.side != null)
                    {
                        TRADE_DATA tempdata = new TRADE_DATA(resData);
                        if (tempdata.CreateTime.Minute == minVal)
                        {
                            dataList.Add(tempdata);
                        }
                        else
                        {
                            if (dataList.Count != 0)
                            {
                                TRADE_INFO_PER_MIN perMinData = new TRADE_INFO_PER_MIN(dataList);
                                Console.WriteLine("per min data record" + JsonConvert.SerializeObject(perMinData));
                                db.TRADE_INFO_PER_MIN.Add(perMinData);
                                db.SaveChanges();
                            }
                            dataList = new List<TRADE_DATA>();
                            minVal = tempdata.CreateTime.Minute;
                        }
                        db.TRADE_DATA.Add(tempdata);
                        db.SaveChanges();
                    }
                }, data, 10).Wait();
            }
        }
    }
}