﻿using System;
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
        List<StockCode> stockList = new List<StockCode>();
        DacStock dacStock = new DacStock();
        bool bDoneStockCodeUpdate = false;
        string inqDate = DateTime.Now.ToString("yyyyMMdd");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axKHOpenAPI1.CommConnect();
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

                        stockList.Add(code);
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

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            try
            {
                Console.WriteLine(e.sScrNo);
                Console.WriteLine(e.sRQName);
                Console.WriteLine(e.sTrCode);
                Console.WriteLine(e.sRecordName);
                Console.WriteLine(e.sPrevNext);

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
                    int rowCount = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                    for ( int i = 0; i < rowCount; i++ )
                    {
                        StockDaily stockInfo = new StockDaily();
                        stockInfo.inqDate = inqDate;
                        stockInfo.stockCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목코드").Trim();
                        stockInfo.stockName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "종목명").Trim();
                        
                        stockInfo.currentPrice = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재가").Trim();
                        if ( !string.IsNullOrWhiteSpace( stockInfo.currentPrice) )
                            stockInfo.currentPrice = Math.Abs(decimal.Parse(stockInfo.currentPrice)).ToString();

                        stockInfo.traffic = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "현재거래량").Trim();
                        stockInfo.diffBefore = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, i, "전일대비").Trim();

                        if (dacStock.GetStockDailyInfo(inqDate, stockList[i].stockCode) == null)
                            dacStock.insertStockDaily(stockInfo);
                    }
                }
                else if (e.sRQName.Equals("계좌별주문체결현황요청"))
                {
                    string count = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "조회건수").Trim();
                    string orderNo = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "주문번호").Trim();

                    int rowCount = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                    MessageBox.Show(orderNo);
                }

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
                int index = stockList.FindIndex(o => o.stockName == stockName);
                string stockCode = stockList[index].stockCode;
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

        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            try
            {
                if (e.sGubun == "0")//주문 접수 , 체결시
                {
                    alertListBox.Items.Add("계좌번호 : " + axKHOpenAPI1.GetChejanData(9201) + " | " + " 주문번호 : " + axKHOpenAPI1.GetChejanData(9203));
                    alertListBox.Items.Add("주문상태 : " + axKHOpenAPI1.GetChejanData(913) + " | " + " 종목명 : " + axKHOpenAPI1.GetChejanData(302));
                    alertListBox.Items.Add("매매구분" + axKHOpenAPI1.GetChejanData(906) + " | " + " 주문수량 : " + axKHOpenAPI1.GetChejanData(900));

                    string orderTime = axKHOpenAPI1.GetChejanData(908);
                    string orderHour = orderTime[0] + "" + orderTime[1];
                    string orderMinute = orderTime[2] + "" + orderTime[3];
                    string orderSecond = orderTime[4] + "" + orderTime[5];
                    long orderPrice = long.Parse(axKHOpenAPI1.GetChejanData(901));

                    alertListBox.Items.Add("주문/체결시간 : " + orderHour + "시 " + orderMinute + "분 " + orderSecond + "초");
                    alertListBox.Items.Add("주문구분 : " + axKHOpenAPI1.GetChejanData(905));
                    alertListBox.Items.Add("주문가격 : " + String.Format("{0:#,###}", orderPrice));
                    alertListBox.Items.Add("----------------------------------------------------------");
                }
            }
            catch (Exception ex)
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
            e.Result = TimeConsumingOperation(bw, arg);

            // If the operation was canceled by the user,
            // set the DoWorkEventArgs.Cancel property to true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        // This method models an operation that may take a long time
        // to run. It can be cancelled, it can raise an exception,
        // or it can exit normally and return a result. These outcomes
        // are chosen randomly.
        private int TimeConsumingOperation(
            BackgroundWorker bw,
            int sleepPeriod)
        {
            int result = 0;

            Random rand = new Random();

            while (!bw.CancellationPending)
            {
                bool exit = false;

                // tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
                if (!bDoneStockCodeUpdate)
                    UpdateStockCode(bw);
                /*
                for (int i = 0; i < stockList.Count; i++)
                {
                    if (dacStock.GetStockDailyInfo(inqDate, stockList[i].stockCode) == null)
                    {
        
                        axKHOpenAPI1.SetInputValue("종목코드", stockList[i].stockCode);
                        axKHOpenAPI1.CommRqData("종목정보요청Job", "opt10001", 0, "5000");
        

                        

                        Thread.Sleep(2000);
                    }

                    bw.ReportProgress(i);


                    if (bw.CancellationPending) break;
                }
        */

                axKHOpenAPI1.SetInputValue("시장구분", "001"); // 000 : 전체, 001: 코스피, 101 : 코스닥
                axKHOpenAPI1.SetInputValue("주기구분", "5");
                axKHOpenAPI1.SetInputValue("거래량구분", "1000");
                axKHOpenAPI1.CommRqData("거래량순조회", "opt10024", 0, "5001");

                Thread.Sleep(sleepPeriod);

                exit = true;

                if (exit)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// tbl_stock_code 를 초기화 및 업데이트를 한다.(하루에 한번)
        /// </summary>
        /// <param name="bw"></param>
        private void UpdateStockCode(BackgroundWorker bw)
        {
            for (int i = 0; i < stockList.Count; i++)
            {
                if (dacStock.GetStockCode(stockList[i]) == null)
                {
                    dacStock.insertStockCode(stockList[i]);
                }

                // (i + 1) / stockList.Count * 100
                bw.ReportProgress(i);

                if (bw.CancellationPending) break;
            }

            bDoneStockCodeUpdate = true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStockCount.Text = e.ProgressPercentage.ToString();
        }

        #endregion

        /// <summary>
        /// 자동 프로그램 매매 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJobStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.backgroundWorker1.RunWorkerAsync(2000);

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
                axKHOpenAPI1.SetInputValue("주문일자", "20200504");

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

                MessageBox.Show(result.ToString());

            } catch ( Exception ex )
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
