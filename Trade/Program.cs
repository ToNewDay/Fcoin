using DataAccess;
using DataAccess.Data;
using DataAccess.Param;
using FcoinUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Trade
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region 获取参数

            string secret = ConfigurationManager.AppSettings["secret"];
            string key = ConfigurationManager.AppSettings["key"];
            double usdtLimit = double.Parse(ConfigurationManager.AppSettings["usdtLimit"]);
            Symbol targetSymbol = UtilFunc.FormtStr(ConfigurationManager.AppSettings["targetSymbol"]);
            Console.WriteLine("Target Symbol:" + ConfigurationManager.AppSettings["targetSymbol"]);
            TimeSpan orderHolder = TimeSpan.FromMinutes(3);

            #endregion 获取参数

            FcoinClient fcilent = new FcoinClient(key, secret);

            List<BalanceInfo> balanceList = new List<BalanceInfo>();
            using (AppDbContext db = new AppDbContext())
            {
                //数据铺地
                //判断是否有订单存在

                //SuperTrade superTrade = null;
                //if (orderInfo == null)
                //{
                //    orderInfo = new TARGET_ORDER()
                //    {
                //        BaseCur = targetSymbol.BaseCurr,
                //        SwapCur = targetSymbol.SwapCurr
                //    };
                //    //List<SuperTrade> targetList = getTarget();
                //    //superTrade = targetList.Where(m => m.SymbolCurr.BaseCurr.ToUpper() == "USDT").OrderBy(m => (m.Reward / (m.DiffData == double.NaN ? 1 : m.DiffData))).FirstOrDefault();
                //    //orderInfo = new TARGET_ORDER()
                //    //{
                //    //    BaseCur = superTrade.SymbolCurr.BaseCurr,
                //    //    SwapCur = superTrade.SymbolCurr.SwapCurr,
                //    //};
                //}

                //订阅行情

                WebSocketUtil webSocketUtil = new WebSocketUtil();

                //服务器时差
                TimeSpan timeSpan = TimeSpan.Zero;
                webSocketUtil.connect(" wss://exchange.ifukang.com/api/web/ws", (string res) =>
                {
                    dynamic data = JsonConvert.DeserializeObject(res);
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    if (data!=null && data.ts != null && data.bids==null)
                    {
                        long wsTicks = dt.Ticks + (long)data.ts * 10000;
                        timeSpan = DateTime.UtcNow - (new DateTime(wsTicks));
                        Console.WriteLine("server time - fcoin time =" + timeSpan.TotalSeconds + "s");
                    }

                    if (data == null || data.bids == null)
                    {
                        Console.WriteLine("ws data error " + res);
                        return;
                    }
                    //计算时间戳，跳过旧行情数据
                    long tt = dt.Ticks + (long)data.ts * 10000;
                    DateTime wsDatetime = new DateTime(tt);
                    if (Math.Abs( (DateTime.UtcNow- wsDatetime).TotalSeconds) >Math.Abs( timeSpan.TotalSeconds)+ 10)
                    {
                        Console.WriteLine("ws time old ,do nothing now: "+(DateTime.UtcNow+TimeSpan.FromHours(8)).ToString("yyyy-MM-dd HH:mm:ss")+" ws time: "+ (wsDatetime+TimeSpan.FromHours(8)).ToString("yyyy-MM-dd HH:mm:ss"));
                        return;
                    }

                    TARGET_ORDER orderInfo = db.TARGET_ORDER.Where(m => m.Completed != "1" && m.BaseCur == targetSymbol.BaseCurr && m.SwapCur == targetSymbol.SwapCurr).OrderByDescending(m => m.Id).FirstOrDefault();


                    if (orderInfo == null)
                    {
                        //还未交易过
                        var balanceListResp = fcilent.GetBalanceList();
                        balanceList = balanceListResp.data;
                        double usdtAmount = double.Parse(balanceList.Where(m => m.currency.ToUpper() == "USDT").FirstOrDefault().available);
                        usdtAmount = usdtAmount < usdtLimit ? usdtAmount : usdtLimit;
                        if (usdtAmount < 50)
                        {
                            Console.WriteLine("usdt small  than 200");
                            return;
                        }
                        double buyPrice = data.bids[6].Value;

                        MakeOrderParam makeOrderParam = new MakeOrderParam()
                        {
                            amount = ((usdtAmount / buyPrice) * 0.99).ToString(),
                            price = buyPrice.ToString(),
                            side = "buy",
                            symbol = (targetSymbol.SwapCurr + targetSymbol.BaseCurr).ToLower()
                        };

                        Console.WriteLine("make order " + JsonConvert.SerializeObject(makeOrderParam));

                        FcoinResponse<string> makerOrderResp = fcilent.MakerOrder(makeOrderParam);
                        Console.WriteLine("make order result " + JsonConvert.SerializeObject(makerOrderResp));
                        if (makerOrderResp.status == "0")
                        {
                            orderInfo = new TARGET_ORDER()
                            {
                                OrderId = makerOrderResp.data,
                                Price = buyPrice,
                                Completed = "0",
                                BaseCur = targetSymbol.BaseCurr,
                                SwapCur = targetSymbol.SwapCurr,
                                CreateTime=DateTime.Now
                            };
                            db.TARGET_ORDER.Add(orderInfo);
                            db.SaveChanges();
                        }
                    }
                    else if (orderInfo.Completed == "0")
                    {
                        //判断订单状态
                        var orderInfoRes = fcilent.GetOrderDetail(orderInfo.OrderId);
                        if (orderInfoRes.status == "0" && orderInfoRes.data.state == "filled")
                        {
                            //若成交则卖出
                            OrderInfo buyOrderInfo = orderInfoRes.data;
                            Console.WriteLine("order filled ,make order to sell ");
                            double sellPrice = double.Parse(buyOrderInfo.price) * 1.005;
                            MakeOrderParam makeOrderParam = new MakeOrderParam()
                            {
                                amount = buyOrderInfo.filled_amount,
                                price = sellPrice.ToString(),
                                side = "sell",
                                symbol = (targetSymbol.SwapCurr + targetSymbol.BaseCurr).ToLower()
                            };
                            Console.WriteLine("make order " + JsonConvert.SerializeObject(makeOrderParam));

                            FcoinResponse<string> makerOrderResp = fcilent.MakerOrder(makeOrderParam);
                            Console.WriteLine("make order result " + JsonConvert.SerializeObject(makerOrderResp));
                            if (makerOrderResp.status == "0")
                            {
                                orderInfo.SellOrderId = makerOrderResp.data;
                                orderInfo.Completed = "2";
                                orderInfo.SwapAmount = double.Parse(buyOrderInfo.amount);
                                db.SaveChanges();
                            }
                            return;
                        }
                        else if (orderInfoRes.status == "0" && orderInfoRes.data.state == "submitted")
                        {
                            //若未成交且价格浮动，则取消订单
                            if (data.bids[2].Value < orderInfo.Price || orderInfo.Price < data.bids[20].Value)
                            {
                                //判断持单时长
                                if (DateTime.Now - orderInfo.CreateTime < orderHolder)
                                {
                                    Console.WriteLine("order holder wait time to  "+((orderInfo.CreateTime+orderHolder).ToUniversalTime()+TimeSpan.FromHours(8)).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                Console.WriteLine("price change ,cancle order ");
                                var cancleResult = fcilent.CancleOrder(orderInfo.OrderId);
                                orderInfo.Completed = "1";
                                db.SaveChanges();
                            }
                        }
                    }
                    else if (orderInfo.Completed == "2")
                    {
                        //判断订单状态
                        var orderInfoRes = fcilent.GetOrderDetail(orderInfo.SellOrderId);
                        OrderInfo sellOrderInfo = orderInfoRes.data;

                        if (orderInfoRes.status == "0" && orderInfoRes.data.state == "filled")
                        {
                            //若成交则置为完成
                            orderInfo.Completed = "1";
                            //等待三分钟，防止追高
                            db.SaveChanges();
                            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(3));
                            //关闭，等待自动重连
                            webSocketUtil.close();
                            return;
                        }
                        else if(orderInfoRes.status == "0" && orderInfoRes.data.state == "submitted")
                        {
                            //判断损失是否超过5%，有则卖出止损
                            if (data.bids[0].Value / orderInfo.Price <0.95)
                            {
                                //取消卖单

                                Console.WriteLine("value loss , sell");
                                double sellPrce = data.bids[0].Value / 2;
                                MakeOrderParam makeOrderParam = new MakeOrderParam()
                                {
                                    amount = orderInfo.SwapAmount.ToString(),
                                    price =sellPrce.ToString(),
                                    side = "sell",
                                    symbol = (orderInfo.SwapCur + orderInfo.BaseCur).ToLower()
                                };
                                Console.WriteLine("make order " + JsonConvert.SerializeObject(makeOrderParam));
                                FcoinResponse<string> makerOrderResp = fcilent.MakerOrder(makeOrderParam);
                                Console.WriteLine("make order result " + JsonConvert.SerializeObject(makerOrderResp));
                                orderInfo.Completed = "1";
                                db.SaveChanges();
                            }
                        }
                    }
                    
                }, new List<Dictionary<string, object>>()
                  {
                      new Dictionary<string, object>()
                      {
                          { "args",new List<string>{ "depth.L20." + targetSymbol.SwapCurr.ToLower()+ targetSymbol.BaseCurr.ToLower() } },
                          { "cmd","sub" }
                      }
                  }, 10).Wait();
            }
        }

        public static List<SuperTrade> getTarget()
        {
            List<REWARD_DATA> rewareList = new List<REWARD_DATA>();
            List<SuperTrade> target = new List<SuperTrade>();
            TimeSpan diffMin = TimeSpan.FromMinutes(20);
            using (AppDbContext db = new AppDbContext())
            {
                rewareList = db.REWARD_DATA.ToList();
                //获取GMT+8当日交易币对总交易量
                List<TRADE_INFO_PER_MIN> tradePerMinData = db.TRADE_INFO_PER_MIN.Where(m => m.Gmt8DataTime.Substring(0, 8) == (DateTime.UtcNow + TimeSpan.FromHours(8)).ToString("yyyyMMdd")).ToList();
                //获取20分钟内交易数据
                List<TRADE_DATA> tradeData = db.TRADE_DATA.Where(m => m.CreateTime > (DateTime.UtcNow + TimeSpan.FromHours(8) - diffMin)).ToList();
                foreach (REWARD_DATA data in rewareList)
                {
                    List<TRADE_DATA> targetTradeData = tradeData.Where(m => m.BaseCur.ToUpper() == data.BaseCur.ToUpper() && m.SwapCur.ToUpper() == data.SwapCur.ToUpper()).ToList();

                    double avg = targetTradeData.Sum(m => m.Price * m.Amount) / targetTradeData.Sum(m => m.Amount);

                    double diffVal = 0;

                    foreach (TRADE_DATA d in targetTradeData)
                    {
                        diffVal += Math.Pow(d.Price - avg, 2);
                    }

                    target.Add(new SuperTrade()
                    {
                        SymbolCurr = new Symbol() { BaseCurr = data.BaseCur.ToUpper(), SwapCurr = data.SwapCur.ToUpper() },
                        DiffData = diffVal / (targetTradeData.Count * avg),
                        Reward = data.Reward,
                        TotalTrade = tradePerMinData.Sum(m => m.TotalTradeBase)
                    });
                }
            }

            return target;
        }
    }
}