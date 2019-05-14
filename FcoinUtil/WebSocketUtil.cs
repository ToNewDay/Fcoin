using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FcoinUtil
{
    public class WebSocketUtil
    {
        private ClientWebSocket web_socket;

        public async Task connect(string url, Action<string> callBack, List<Dictionary<string, object>> sendMsgList, int pingSecond)
        {
            await connectWebSocket(url, sendMsgList);
            while (true)
            {
                DateTime startTime = DateTime.Now;
                try
                {
                    if (web_socket.CloseStatus == WebSocketCloseStatus.Empty)
                    {
                        break;
                    }
                    ArraySegment<byte> byte_received = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult json_notify_result = await web_socket.ReceiveAsync(byte_received, CancellationToken.None);
                    string string_notify = Encoding.UTF8.GetString(byte_received.Array);
                    callBack(string_notify);
                    if (DateTime.Now - startTime >= TimeSpan.FromSeconds(pingSecond))
                    {
                        startTime = DateTime.Now;
                        Dictionary<string, object> pingmsg = new Dictionary<string, object>()
                        {
                            {"args",new  List<int>(){  (int)Math.Floor((DateTime.UtcNow- new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc)).TotalMilliseconds)} },
                            {"cmd","ping" },
                        };
                        string pingmsgStr = JsonConvert.SerializeObject(pingmsg);
                        ArraySegment<byte> sendPingbyte = new ArraySegment<byte>(Encoding.UTF8.GetBytes(pingmsgStr));
                        await web_socket.SendAsync(sendPingbyte, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception" + ex.ToString());
                    Console.WriteLine("reconnect");
                    await connect(url, callBack, sendMsgList, pingSecond);
                }
            }
        }

        public void close()
        {
            web_socket.CloseAsync(WebSocketCloseStatus.Empty,"close", CancellationToken.None).Wait();
        }

        public async Task connectWebSocket(string url,   List<Dictionary<string, object>> sendMsgList)
        {
            web_socket = new ClientWebSocket();
            await web_socket.ConnectAsync(new Uri(url), CancellationToken.None);
            foreach (var sendMsg in sendMsgList)
            {
                string initMsgStr = JsonConvert.SerializeObject(sendMsg);
                ArraySegment<byte> sendbyte = new ArraySegment<byte>(Encoding.UTF8.GetBytes(initMsgStr));
                await web_socket.SendAsync(sendbyte, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}