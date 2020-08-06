using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2.Common;
using WindowsFormsApp2.Dac;
using WindowsFormsApp2.Data;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Biz biz = null;
        TRBiz trBiz = null;
        SubBiz subBiz = null;
        DacStock dacStock = new DacStock();
        string inqDate = DateTime.Now.ToString("yyyyMMdd");
        //string orderDate = "20200517";
        APIManager apiManager = null;
        public string ExecuteMode { get; set; }

        public Form1( string mode)
        {
            InitializeComponent();
            ExecuteMode = mode;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            apiManager = new APIManager(axKHOpenAPI1);
            axKHOpenAPI1.CommConnect();
            trBiz = new TRBiz(apiManager);
            subBiz = new SubBiz(apiManager);
            biz = new Biz(apiManager, subBiz, trBiz);
        }

        private void axKHOpenAPI1_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            try
            {
                if (e.nErrCode == 0)
                {
                    string accountlist = axKHOpenAPI1.GetLoginInfo("ACCLIST");
                    string[] account = accountlist.Split(';');
                    for (int i = 0; i < account.Length; i++)
                    {
                        comboBox1.Items.Add(account[i]);
                    }

                    if ( account.Length > 0 )
                        comboBox1.SelectedIndex = 0;

                    string userId = axKHOpenAPI1.GetLoginInfo("USER_ID");
                    string userName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                    string connectedServer = axKHOpenAPI1.GetLoginInfo("GetServerGubun");
                    lblUserID.Text = userId;
                    lblUserName.Text = userName;
                    lblServerNo.Text = connectedServer;

                    string stockCode = axKHOpenAPI1.GetCodeListByMarket("0");
                    string[] stockCodeArray = stockCode.Split(';');

                    lblCountOfStocks.Text = stockCodeArray.Length.ToString();

                    for (int i = 0; i < stockCodeArray.Length; i++)
                    {
                        StockCode code = new StockCode();
                        code.stockCode = stockCodeArray[i];
                        code.stockName = axKHOpenAPI1.GetMasterCodeName(stockCodeArray[i]);

                        biz.stockList.Add(code);
                    }
                    
                    Biz.AccountNo = comboBox1.SelectedItem.ToString();

                    if ( string.IsNullOrEmpty( Biz.AccountNo ) )
                    {
                        MessageBox.Show("계좌가 존재하지 않음!!!");
                        return;
                    }

                    if ( "Auto".Equals( ExecuteMode ))
                    {
                        button3_Click(null, null);

                        this.backgroundWorker1.RunWorkerAsync(10000);
                    }
                    else if ( "SetTarget".Equals( ExecuteMode ) )
                    {
                        trBiz.당일거래량상위요청("1", "3");
                    }
                }
                else
                    MessageBox.Show("연결오류");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //axKHOpenAPI1.CommConnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            try
            {
                axKHOpenAPI1.SetInputValue("계좌번호", comboBox1.SelectedItem.ToString());
                axKHOpenAPI1.SetInputValue("비밀번호", txtPassword.Text);
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
                axKHOpenAPI1.SetInputValue("조회구분", "1");

                axKHOpenAPI1.CommRqData("계좌평가잔고내역요청", "opw00018", 0, "8100");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                axKHOpenAPI1.SetInputValue("계좌번호", comboBox1.SelectedItem.ToString());
                axKHOpenAPI1.SetInputValue("비밀번호", txtPassword.Text);
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
                axKHOpenAPI1.SetInputValue("조회구분", "1");

                axKHOpenAPI1.CommRqData("예수금상세현황요청", "opw00001", 0, "8100");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string currentStockCode;

        private void btnStockInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                string stockName = txtStockName.Text;
                int index = biz.stockList.FindIndex(o => o.stockName == stockName);
                string stockCode = biz.stockList[index].stockCode;
                currentStockCode = stockCode;

                axKHOpenAPI1.SetInputValue("종목코드", stockCode);
                axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            try
            {
                // 계좌번호
                string accountCode = comboBox1.SelectedItem.ToString();

                // 종목코드
                string orderCode = currentStockCode;

                int stockQty = int.Parse(txtStockQty.Text);
                int stockPrice = int.Parse(txtStockPrice.Text);

                int resultCode = axKHOpenAPI1.SendOrder("종목신규매수", "8249", accountCode, 1, orderCode, stockQty, stockPrice, "00", "");

                MessageBox.Show(resultCode.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            try
            {
                // 계좌번호
                string accountCode = comboBox1.SelectedItem.ToString();

                // 종목코드
                string orderCode = currentStockCode;

                int stockQty = int.Parse(txtStockQty.Text);
                int stockPrice = int.Parse(txtStockPrice.Text);

                int resultCode = axKHOpenAPI1.SendOrder("신규종목매도주문", "8289", accountCode, 2, orderCode, stockQty, stockPrice, "00", "");

                MessageBox.Show(resultCode.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            try
            {
                log.Info("axKHOpenAPI1_OnReceiveTrData start e.sScrNo:" + e.sScrNo + " e.sRQName:" + e.sRQName + " e.sTrCode:" + e.sTrCode
                    + " e.sRecordName:" + e.sRecordName + " e.sPrevNext:" + e.sPrevNext );

                if (e.sRQName == "계좌평가잔고내역요청")
                {
                    long totalPurchase = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액"));
                    long totalEstimate = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가금액"));
                    long totalProfitLoss = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가손익금액"));
                    double totalProfitRate = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총수익률(%)"));

                    totalProfitRateLabel.Text = String.Format("{0:#,###}", totalPurchase);
                    totalEstimateLabel.Text = String.Format("{0:#,###}", totalEstimate);
                    totalProfitLabel.Text = String.Format("{0:#,###}", totalProfitLoss);
                    lblProfitRate.Text = String.Format("{0:f2}", totalProfitRate);
                }
                else if (e.sRQName.Equals("예수금상세현황요청"))
                {
                    long lBalance = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "예수금"));
                    string strBalance = String.Format("{0:#,###}", lBalance);
                    MessageBox.Show(strBalance);
                }
                else if (e.sRQName.Equals("종목정보요청"))
                {
                    long stockPrice = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").Trim().Replace("-", ""));
                    string stockName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                    long upDown = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비").Trim());
                    long volume = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "거래량").Trim());
                    string upDownRate = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "등락율").Trim();

                    lblCurrentPrice.Text = String.Format("{0:#,###}", stockPrice);
                    lblStockName.Text = stockName;
                    lblDiffPrice.Text = String.Format("{0:#,###}", upDown);
                    lblVolume.Text = String.Format("{0:#,###}", volume);
                    if (upDown == 0)
                    {
                        lblDiffPrice.Text = "0";
                    }
                    if (volume == 0)
                    {
                        lblDiffPrice.Text = "0";
                    }
                    lblUpDownRate.Text = upDownRate + "%";

                    StockDaily stockInfo = new StockDaily();
                    stockInfo.inqDate = inqDate;

                    dacStock.insertStockDaily(stockInfo);
                }
                else if (e.sRQName.Equals("종목정보요청Job"))
                {
                    string currentPrice = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").Trim().Replace("-", "");
                    long stockPrice = long.Parse(string.IsNullOrEmpty(currentPrice.Trim()) ? "0" : currentPrice.Trim());
                    string stockCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();

                    if (!string.IsNullOrWhiteSpace(stockCode))
                    {
                        string stockName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                        long upDown = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비").Trim());
                        long volume = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "거래량").Trim());
                        string upDownRate = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "등락율").Trim();

                        StockDaily stockInfo = new StockDaily();
                        stockInfo.inqDate = inqDate;
                        stockInfo.stockCode = stockCode;
                        stockInfo.stockName = stockName;
                        stockInfo.currentPrice = stockPrice.ToString();
                        stockInfo.diffBefore = upDown.ToString();
                        stockInfo.traffic = volume.ToString();
                        stockInfo.upDownRate = upDownRate;

                        dacStock.insertStockDaily(stockInfo);
                    }
                }
                else if (e.sRQName.Equals("거래량순조회"))
                {
                    trBiz.거래량순리스트조회처리(sender, e);
                }
                else if (e.sRQName.Equals("계좌별주문체결현황요청"))
                {
                    trBiz.계좌별주문체결현황요청응답처리(inqDate, sender, e, e.sPrevNext);
                }
                else if (e.sRQName.Equals("종목신규매수") || e.sRQName.Equals("신규종목매도주문") || e.sRQName.Equals("매도취소요청"))
                {
                    string orderSeq = e.sScrNo;
                    string orderNo = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "주문번호").Trim();
                    string stockCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();

                    log.Info(e.sRQName + " 주문번호: " + orderNo + " orderSeq:" + orderSeq + " stockCode:" + stockCode );

                    //if (string.IsNullOrWhiteSpace(orderNo))
                    //{
                    //    log.Error("주문번호가 없어 씨벌");
                    //}
                    //else
                    //{
                    //    log.Info("주문번호업데이트!! orderSeq: " + orderSeq + " orderNo:" + orderNo);
                    //    dacStock.주문번호업데이트_bySeq(orderSeq, orderNo);
                    //}
                }
                else if (e.sRQName.Equals("매도정정요청"))
                {
                    //string orderSeq = e.sScrNo;
                    //string orderNo = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "주문번호").Trim();
                    //string stockCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드").Trim();

                    //log.Info(e.sRQName + " 주문번호: " + orderNo + " orderSeq:" + orderSeq + " stockCode:" + stockCode);

                    //dacStock.주문번호업데이트_bySeq(orderSeq, orderNo);
                }
                else if (e.sRQName.Equals("계좌평가현황요청"))
                {
                    int rowCount = apiManager.GetRepeatCnt(e.sTrCode, e.sRQName);

                    for (int i = 0; i < rowCount; i++)
                    {
                        StockOrder stockInfo = new StockOrder();
                        stockInfo.inqDate = inqDate;
                        stockInfo.stockCode = apiManager.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                        stockInfo.stockName = apiManager.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();

                        if (stockInfo.stockCode.StartsWith("A"))
                            stockInfo.stockCode = stockInfo.stockCode.Substring(1);

                        stockInfo.Price = apiManager.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                        if (string.IsNullOrWhiteSpace(stockInfo.Price))
                            stockInfo.Price = "0";

                        int price = int.Parse(stockInfo.Price); // - (int)(int.Parse(stockInfo.Price) * 0.02);

                        stockInfo.Qty = apiManager.GetCommData(e.sTrCode, e.sRQName, i, "보유수량").Trim();

                        if (string.IsNullOrWhiteSpace(stockInfo.Qty))
                            stockInfo.Qty = "0";

                        int qty = int.Parse(stockInfo.Qty);

                        apiManager.SendOrder("매도처리", "1234", comboBox1.SelectedItem.ToString(), 2, stockInfo.stockCode, qty, price, "00", "");

                        Thread.Sleep(500);

                        log.Info("잔고정리:" + JsonConvert.SerializeObject(stockInfo));
                    }

                }
                else if (e.sRQName.Equals("실시간미체결요청"))
                {
                    trBiz.실시간미체결요청응답처리(sender, e);
                }
                else if (e.sRQName.Equals("당일거래량순조회") || "당일거래대금순조회".Equals(e.sRQName))
                {
                    trBiz.당일거래량상위응답처리(sender, e);
                }
                else if (e.sRQName.Equals("전일거래량순조회") || "전일거래대금순조회".Equals(e.sRQName))
                {
                    trBiz.전일거래량상위응답처리(sender, e);
                }
                else if (e.sRQName.Equals("종목현재가조회"))
                {
                    trBiz.종목현재가조회응답처리(sender, e);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                log.Info("axKHOpenAPI1_OnReceiveTrData end");
            }
        }
        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            try
            {
                log.Info("axKHOpenAPI1_OnReceiveChejanData start");

                if (e.sGubun == "0")//주문 접수 , 체결시
                {
                    //alertListBox.Items.Add("계좌번호 : " + axKHOpenAPI1.GetChejanData(9201) + " | " + " 주문번호 : " + axKHOpenAPI1.GetChejanData(9203));
                    //alertListBox.Items.Add("주문상태 : " + axKHOpenAPI1.GetChejanData(913) + " | " + " 종목명 : " + axKHOpenAPI1.GetChejanData(302));
                    //alertListBox.Items.Add("종목코드 : " + axKHOpenAPI1.GetChejanData(9001) + " | " + " 원주문번호 : " + axKHOpenAPI1.GetChejanData(904));
                    //alertListBox.Items.Add("매매구분" + axKHOpenAPI1.GetChejanData(906) + " | " + " 주문수량 : " + axKHOpenAPI1.GetChejanData(900));

                    string orderTime = axKHOpenAPI1.GetChejanData(908);
                    string orderHour = orderTime[0] + "" + orderTime[1];
                    string orderMinute = orderTime[2] + "" + orderTime[3];
                    string orderSecond = orderTime[4] + "" + orderTime[5];
                    long orderPrice = long.Parse(axKHOpenAPI1.GetChejanData(901));

                    //alertListBox.Items.Add("주문/체결시간 : " + orderHour + "시 " + orderMinute + "분 " + orderSecond + "초");
                    //alertListBox.Items.Add("주문구분 : " + axKHOpenAPI1.GetChejanData(905));
                    //alertListBox.Items.Add("주문가격 : " + String.Format("{0:#,###}", orderPrice));
                    //alertListBox.Items.Add("----------------------------------------------------------");

                    log.Info("계좌번호 : " + axKHOpenAPI1.GetChejanData(9201) + " | " + " 주문번호 : " + axKHOpenAPI1.GetChejanData(9203));
                    log.Info("주문상태 : " + axKHOpenAPI1.GetChejanData(913) + " | " + " 종목명 : " + axKHOpenAPI1.GetChejanData(302));
                    log.Info("종목코드 : " + axKHOpenAPI1.GetChejanData(9001) + " | " + " 원주문번호 : " + axKHOpenAPI1.GetChejanData(904));
                    log.Info("매매구분" + axKHOpenAPI1.GetChejanData(906) + " | " + " 주문수량 : " + axKHOpenAPI1.GetChejanData(900));
                    log.Info("주문/체결시간 : " + orderHour + "시 " + orderMinute + "분 " + orderSecond + "초");
                    log.Info("주문구분 : " + axKHOpenAPI1.GetChejanData(905));
                    log.Info("주문가격 : " + String.Format("{0:#,###}", orderPrice));
                    log.Info("체결수량 : " + axKHOpenAPI1.GetChejanData(911) + " | " + " 체결가 : " + axKHOpenAPI1.GetChejanData(910));
                    log.Info("----------------------------------------------------------");

                    string stockCode = axKHOpenAPI1.GetChejanData(9001).Trim();
                    string 주문구분 = axKHOpenAPI1.GetChejanData(905).Trim();
                    string 주문번호 = axKHOpenAPI1.GetChejanData(9203).Trim();
                    string 주문수량 = axKHOpenAPI1.GetChejanData(900).Trim();
                    string 주문가격 = axKHOpenAPI1.GetChejanData(901).Trim();

                    if (주문구분.IndexOf("매수취소") >= 0)
                        주문구분 = "매수취소";
                    else if (주문구분.IndexOf("매수") >= 0)
                        주문구분 = "매수";
                    else if (주문구분.IndexOf("매도정정") >= 0)
                        주문구분 = "매도정정";
                    else if (주문구분.IndexOf("매도취소") >= 0)
                        주문구분 = "매도취소";
                    else if (주문구분.IndexOf("매도") >= 0)
                        주문구분 = "매도";
                    else
                        주문구분 = string.Empty;

                    if (stockCode.StartsWith("A"))
                        stockCode = stockCode.Substring(1);

                    if ("체결".Equals(axKHOpenAPI1.GetChejanData(913).Trim()))
                    {
                        string 체결수량 = axKHOpenAPI1.GetChejanData(911).Trim();
                        string 체결가 = axKHOpenAPI1.GetChejanData(910).Trim();

                        

                        log.Info("체결인지함. 주문구분:" + 주문구분 + " stockCode:" + stockCode);

                        if ( !string.IsNullOrWhiteSpace(주문구분))
                            subBiz.체결완료처리(inqDate, stockCode, 주문구분, 주문번호 , 주문수량, 주문가격, 체결수량, 체결가);
                    }
                    else if ("접수".Equals(axKHOpenAPI1.GetChejanData(913).Trim()))
                    {
                        List<StockOrder> listOrders = dacStock.tbl_stock_order_주문조회(inqDate, stockCode, 주문구분, "요청중");

                        for ( int i = 0; i < listOrders.Count; i++ )
                        {
                            StockOrder order = listOrders[i];

                            log.Info("접수상태 주문비교 : " + JsonConvert.SerializeObject(order));

                            if (string.IsNullOrWhiteSpace(order.orderNo) &&
                                Util.GetInt(주문수량) == Util.GetInt(order.Qty) && Util.GetInt(주문가격) == Util.GetInt(order.Price))
                            {
                                order.orderNo = 주문번호;
                                dacStock.주문정보업데이트_byOrderSeq(order);

                                log.Info("주문번호 업데이트 !! orderSeq:" + order.Seq + " orderNo:" + order.orderNo);
                            }
                            else
                                log.Info("주문다름");
                        }
                    }
                    else
                    {
                        log.Info("체결아님");
                    }

                    //string stockCode = axKHOpenAPI1.GetChejanData(9001).Trim();
                    //if (stockCode.StartsWith("A"))
                    //    stockCode = stockCode.Substring(1);

                    //string orderType = axKHOpenAPI1.GetChejanData(905).Trim().Replace("+","").Replace("-","");
                    //string qty = axKHOpenAPI1.GetChejanData(900).Trim();
                    //string price = axKHOpenAPI1.GetChejanData(901).Trim();
                    //string orderNo = axKHOpenAPI1.GetChejanData(9203).Trim();

                    //if (axKHOpenAPI1.GetChejanData(913).Trim().Equals("접수"))
                    //{
                    //    log.Info("주문번호업데이트 : " + inqDate + " 종목:" + stockCode + " 수량:" + qty + " 가격:" + price );
                    //    dacStock.주문번호업데이트(inqDate, stockCode, orderType, orderNo, qty, price );
                    //}
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                log.Info("axKHOpenAPI1_OnReceiveChejanData end");
            }
        }

        

        private void axKHOpenAPI1_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            try
            {
                log.Info("axKHOpenAPI1_OnReceiveMsg start");
                log.Info("sRQName:" + e.sRQName + " sTrCode:" + e.sTrCode + " sScrNo:" + e.sScrNo + " sMsg:" + e.sMsg );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                log.Info("axKHOpenAPI1_OnReceiveMsg end");
            }
        }

        /// <summary>
        /// 자동 프로그램 매매 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJobStart_Click(object sender, EventArgs e)
        {
            try
            {
                if ( string.IsNullOrWhiteSpace(Biz.AccountNo))
                {
                    if ( comboBox1.SelectedItem == null || string.IsNullOrWhiteSpace(comboBox1.SelectedItem.ToString() ))
                    {
                        MessageBox.Show("계좌를 선택해 주십시오.");
                        return;
                    }

                    Biz.AccountNo = comboBox1.SelectedItem.ToString();

                    if ( string.IsNullOrWhiteSpace( Biz.AccountNo ) )
                    {
                        MessageBox.Show("계좌번호가 공백입니다.");
                        return;
                    }
                }

                this.backgroundWorker1.RunWorkerAsync(10000);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 자동 프로그램 매매 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJobStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.backgroundWorker1.CancelAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 주문일자 = YYYYMMDD (20170101 연도4자리, 월 2자리, 일 2자리 형식)
                axKHOpenAPI1.SetInputValue("주문일자", inqDate);

                // 계좌번호 = 전문 조회할 보유계좌번호
                axKHOpenAPI1.SetInputValue("계좌번호", comboBox1.SelectedItem.ToString());

                //비밀번호 = 사용안함(공백)
                axKHOpenAPI1.SetInputValue("비밀번호", "");

                // 비밀번호입력매체구분 = 00
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

                // 주식채권구분 = 0:전체, 1:주식, 2:채권
                axKHOpenAPI1.SetInputValue("주식채권구분", "0");

                // 시장구분 = 0:전체, 1:장내, 2:코스닥, 3:OTCBB, 4:ECN
                axKHOpenAPI1.SetInputValue("시장구분", "0");

                // 매도수구분 = 0:전체, 1:매도, 2:매수
                axKHOpenAPI1.SetInputValue("매도수구분", "0");

                // 조회구분 = 0:전체, 1:체결
                axKHOpenAPI1.SetInputValue("조회구분", "0");

                axKHOpenAPI1.SetInputValue("종목코드", "005930");
                axKHOpenAPI1.SetInputValue("시작주문번호", "");

                int result = axKHOpenAPI1.CommRqData("계좌별주문체결현황요청", "opw00009", 0, "1234");


            } catch ( Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region 백그라운드 잡
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;

            // Extract the argument.
            int arg = (int)e.Argument;

            // Start the time-consuming operation.
            e.Result = biz.TimeConsumingOperation(bw, arg);

            // If the operation was canceled by the user,
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStockCount.Text = e.ProgressPercentage.ToString();
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                // 계좌번호 = 전문 조회할 보유계좌번호
                axKHOpenAPI1.SetInputValue("계좌번호", comboBox1.SelectedItem.ToString());

                //비밀번호 = 사용안함(공백)
                axKHOpenAPI1.SetInputValue("비밀번호", "");

                // 상장폐지조회구분 = 0:전체, 1:상장폐지종목제외
                axKHOpenAPI1.SetInputValue("상장폐지조회구분", "0");

                // 비밀번호입력매체구분 = 00
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");


                int result = apiManager.CommRqData("계좌평가현황요청", "OPW00004", 0, "1234");
            } catch ( Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTestForm_Click(object sender, EventArgs e)
        {
            TestForm form = new TestForm();
            form.OpenAPI = axKHOpenAPI1;
            form.trBiz = trBiz;
            form.biz = biz;
            form.Show();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
