namespace WindowsFormsApp2
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserID = new System.Windows.Forms.Label();
            this.lblServerNo = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnGetBalance = new System.Windows.Forms.Button();
            this.totalProfitRateLabel = new System.Windows.Forms.Label();
            this.totalEstimateLabel = new System.Windows.Forms.Label();
            this.totalProfitLabel = new System.Windows.Forms.Label();
            this.lblProfitRate = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtStockName = new System.Windows.Forms.TextBox();
            this.btnStockInquiry = new System.Windows.Forms.Button();
            this.lblStockName = new System.Windows.Forms.Label();
            this.lblCurrentPrice = new System.Windows.Forms.Label();
            this.lblCountOfStocks = new System.Windows.Forms.Label();
            this.lblDiffPrice = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblUpDownRate = new System.Windows.Forms.Label();
            this.btnBuy = new System.Windows.Forms.Button();
            this.txtStockPrice = new System.Windows.Forms.TextBox();
            this.txtStockQty = new System.Windows.Forms.TextBox();
            this.btnSell = new System.Windows.Forms.Button();
            this.alertListBox = new System.Windows.Forms.ListBox();
            this.btnJobStart = new System.Windows.Forms.Button();
            this.btnJobStop = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblStockCount = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.SuspendLayout();
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(875, 486);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(122, 51);
            this.axKHOpenAPI1.TabIndex = 0;
            this.axKHOpenAPI1.OnReceiveTrData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEventHandler(this.axKHOpenAPI1_OnReceiveTrData);
            this.axKHOpenAPI1.OnReceiveChejanData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEventHandler(this.axKHOpenAPI1_OnReceiveChejanData);
            this.axKHOpenAPI1.OnEventConnect += new AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEventHandler(this.axKHOpenAPI1_OnEventConnect);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(380, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(169, 20);
            this.comboBox1.TabIndex = 1;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(380, 39);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(53, 12);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "사용자명";
            // 
            // lblUserID
            // 
            this.lblUserID.AutoSize = true;
            this.lblUserID.Location = new System.Drawing.Point(380, 65);
            this.lblUserID.Name = "lblUserID";
            this.lblUserID.Size = new System.Drawing.Size(41, 12);
            this.lblUserID.TabIndex = 3;
            this.lblUserID.Text = "아이디";
            // 
            // lblServerNo
            // 
            this.lblServerNo.AutoSize = true;
            this.lblServerNo.Location = new System.Drawing.Point(378, 90);
            this.lblServerNo.Name = "lblServerNo";
            this.lblServerNo.Size = new System.Drawing.Size(53, 12);
            this.lblServerNo.TabIndex = 4;
            this.lblServerNo.Text = "서버구분";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(13, 8);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(565, 10);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 21);
            this.txtPassword.TabIndex = 6;
            // 
            // btnGetBalance
            // 
            this.btnGetBalance.Location = new System.Drawing.Point(671, 10);
            this.btnGetBalance.Name = "btnGetBalance";
            this.btnGetBalance.Size = new System.Drawing.Size(158, 22);
            this.btnGetBalance.TabIndex = 7;
            this.btnGetBalance.Text = "계좌평가잔고내역요청";
            this.btnGetBalance.UseVisualStyleBackColor = true;
            this.btnGetBalance.Click += new System.EventHandler(this.btnGetBalance_Click);
            // 
            // totalProfitRateLabel
            // 
            this.totalProfitRateLabel.AutoSize = true;
            this.totalProfitRateLabel.Location = new System.Drawing.Point(669, 39);
            this.totalProfitRateLabel.Name = "totalProfitRateLabel";
            this.totalProfitRateLabel.Size = new System.Drawing.Size(65, 12);
            this.totalProfitRateLabel.TabIndex = 8;
            this.totalProfitRateLabel.Text = "총매입금액";
            // 
            // totalEstimateLabel
            // 
            this.totalEstimateLabel.AutoSize = true;
            this.totalEstimateLabel.Location = new System.Drawing.Point(669, 65);
            this.totalEstimateLabel.Name = "totalEstimateLabel";
            this.totalEstimateLabel.Size = new System.Drawing.Size(65, 12);
            this.totalEstimateLabel.TabIndex = 9;
            this.totalEstimateLabel.Text = "총평가금액";
            // 
            // totalProfitLabel
            // 
            this.totalProfitLabel.AutoSize = true;
            this.totalProfitLabel.Location = new System.Drawing.Point(669, 90);
            this.totalProfitLabel.Name = "totalProfitLabel";
            this.totalProfitLabel.Size = new System.Drawing.Size(89, 12);
            this.totalProfitLabel.TabIndex = 10;
            this.totalProfitLabel.Text = "총평가손익금액";
            // 
            // lblProfitRate
            // 
            this.lblProfitRate.AutoSize = true;
            this.lblProfitRate.Location = new System.Drawing.Point(669, 114);
            this.lblProfitRate.Name = "lblProfitRate";
            this.lblProfitRate.Size = new System.Drawing.Size(53, 12);
            this.lblProfitRate.TabIndex = 11;
            this.lblProfitRate.Text = "총수익률";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(845, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 19);
            this.button1.TabIndex = 12;
            this.button1.Text = "예수금상세현황요청";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtStockName
            // 
            this.txtStockName.Location = new System.Drawing.Point(21, 201);
            this.txtStockName.Name = "txtStockName";
            this.txtStockName.Size = new System.Drawing.Size(136, 21);
            this.txtStockName.TabIndex = 13;
            // 
            // btnStockInquiry
            // 
            this.btnStockInquiry.Location = new System.Drawing.Point(172, 203);
            this.btnStockInquiry.Name = "btnStockInquiry";
            this.btnStockInquiry.Size = new System.Drawing.Size(152, 19);
            this.btnStockInquiry.TabIndex = 14;
            this.btnStockInquiry.Text = "종목검색";
            this.btnStockInquiry.UseVisualStyleBackColor = true;
            this.btnStockInquiry.Click += new System.EventHandler(this.btnStockInquiry_Click);
            // 
            // lblStockName
            // 
            this.lblStockName.AutoSize = true;
            this.lblStockName.Location = new System.Drawing.Point(23, 241);
            this.lblStockName.Name = "lblStockName";
            this.lblStockName.Size = new System.Drawing.Size(41, 12);
            this.lblStockName.TabIndex = 15;
            this.lblStockName.Text = "종목명";
            // 
            // lblCurrentPrice
            // 
            this.lblCurrentPrice.AutoSize = true;
            this.lblCurrentPrice.Location = new System.Drawing.Point(23, 267);
            this.lblCurrentPrice.Name = "lblCurrentPrice";
            this.lblCurrentPrice.Size = new System.Drawing.Size(41, 12);
            this.lblCurrentPrice.TabIndex = 15;
            this.lblCurrentPrice.Text = "현재가";
            // 
            // lblCountOfStocks
            // 
            this.lblCountOfStocks.AutoSize = true;
            this.lblCountOfStocks.Location = new System.Drawing.Point(11, 50);
            this.lblCountOfStocks.Name = "lblCountOfStocks";
            this.lblCountOfStocks.Size = new System.Drawing.Size(41, 12);
            this.lblCountOfStocks.TabIndex = 15;
            this.lblCountOfStocks.Text = "종목수";
            // 
            // lblDiffPrice
            // 
            this.lblDiffPrice.AutoSize = true;
            this.lblDiffPrice.Location = new System.Drawing.Point(23, 292);
            this.lblDiffPrice.Name = "lblDiffPrice";
            this.lblDiffPrice.Size = new System.Drawing.Size(53, 12);
            this.lblDiffPrice.TabIndex = 15;
            this.lblDiffPrice.Text = "전일대비";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(23, 317);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(41, 12);
            this.lblVolume.TabIndex = 15;
            this.lblVolume.Text = "거래량";
            // 
            // lblUpDownRate
            // 
            this.lblUpDownRate.AutoSize = true;
            this.lblUpDownRate.Location = new System.Drawing.Point(23, 345);
            this.lblUpDownRate.Name = "lblUpDownRate";
            this.lblUpDownRate.Size = new System.Drawing.Size(41, 12);
            this.lblUpDownRate.TabIndex = 15;
            this.lblUpDownRate.Text = "등락율";
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(172, 285);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(152, 19);
            this.btnBuy.TabIndex = 16;
            this.btnBuy.Text = "매수";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // txtStockPrice
            // 
            this.txtStockPrice.Location = new System.Drawing.Point(172, 258);
            this.txtStockPrice.Name = "txtStockPrice";
            this.txtStockPrice.Size = new System.Drawing.Size(100, 21);
            this.txtStockPrice.TabIndex = 17;
            this.txtStockPrice.Text = "매수가격";
            // 
            // txtStockQty
            // 
            this.txtStockQty.Location = new System.Drawing.Point(172, 232);
            this.txtStockQty.Name = "txtStockQty";
            this.txtStockQty.Size = new System.Drawing.Size(100, 21);
            this.txtStockQty.TabIndex = 18;
            this.txtStockQty.Text = "매수 수량";
            // 
            // btnSell
            // 
            this.btnSell.Location = new System.Drawing.Point(172, 310);
            this.btnSell.Name = "btnSell";
            this.btnSell.Size = new System.Drawing.Size(152, 19);
            this.btnSell.TabIndex = 19;
            this.btnSell.Text = "매도";
            this.btnSell.UseVisualStyleBackColor = true;
            this.btnSell.Click += new System.EventHandler(this.btnSell_Click);
            // 
            // alertListBox
            // 
            this.alertListBox.FormattingEnabled = true;
            this.alertListBox.ItemHeight = 12;
            this.alertListBox.Location = new System.Drawing.Point(410, 201);
            this.alertListBox.Name = "alertListBox";
            this.alertListBox.Size = new System.Drawing.Size(523, 232);
            this.alertListBox.TabIndex = 20;
            // 
            // btnJobStart
            // 
            this.btnJobStart.Location = new System.Drawing.Point(137, 8);
            this.btnJobStart.Name = "btnJobStart";
            this.btnJobStart.Size = new System.Drawing.Size(135, 23);
            this.btnJobStart.TabIndex = 21;
            this.btnJobStart.Text = "자동매매시작";
            this.btnJobStart.UseVisualStyleBackColor = true;
            this.btnJobStart.Click += new System.EventHandler(this.btnJobStart_Click);
            // 
            // btnJobStop
            // 
            this.btnJobStop.Location = new System.Drawing.Point(137, 37);
            this.btnJobStop.Name = "btnJobStop";
            this.btnJobStop.Size = new System.Drawing.Size(135, 23);
            this.btnJobStop.TabIndex = 22;
            this.btnJobStop.Text = "자동매매종료";
            this.btnJobStop.UseVisualStyleBackColor = true;
            this.btnJobStop.Click += new System.EventHandler(this.btnJobStop_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // lblStockCount
            // 
            this.lblStockCount.AutoSize = true;
            this.lblStockCount.Location = new System.Drawing.Point(12, 75);
            this.lblStockCount.Name = "lblStockCount";
            this.lblStockCount.Size = new System.Drawing.Size(41, 12);
            this.lblStockCount.TabIndex = 23;
            this.lblStockCount.Text = "로딩수";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(25, 414);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 19);
            this.button2.TabIndex = 24;
            this.button2.Text = "매도";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 648);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblStockCount);
            this.Controls.Add(this.btnJobStop);
            this.Controls.Add(this.btnJobStart);
            this.Controls.Add(this.alertListBox);
            this.Controls.Add(this.btnSell);
            this.Controls.Add(this.txtStockQty);
            this.Controls.Add(this.txtStockPrice);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.lblUpDownRate);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.lblDiffPrice);
            this.Controls.Add(this.lblCurrentPrice);
            this.Controls.Add(this.lblCountOfStocks);
            this.Controls.Add(this.lblStockName);
            this.Controls.Add(this.btnStockInquiry);
            this.Controls.Add(this.txtStockName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblProfitRate);
            this.Controls.Add(this.totalProfitLabel);
            this.Controls.Add(this.totalEstimateLabel);
            this.Controls.Add(this.totalProfitRateLabel);
            this.Controls.Add(this.btnGetBalance);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblServerNo);
            this.Controls.Add(this.lblUserID);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.axKHOpenAPI1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserID;
        private System.Windows.Forms.Label lblServerNo;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnGetBalance;
        private System.Windows.Forms.Label totalProfitRateLabel;
        private System.Windows.Forms.Label totalEstimateLabel;
        private System.Windows.Forms.Label totalProfitLabel;
        private System.Windows.Forms.Label lblProfitRate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtStockName;
        private System.Windows.Forms.Button btnStockInquiry;
        private System.Windows.Forms.Label lblStockName;
        private System.Windows.Forms.Label lblCurrentPrice;
        private System.Windows.Forms.Label lblCountOfStocks;
        private System.Windows.Forms.Label lblDiffPrice;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblUpDownRate;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.TextBox txtStockPrice;
        private System.Windows.Forms.TextBox txtStockQty;
        private System.Windows.Forms.Button btnSell;
        private System.Windows.Forms.ListBox alertListBox;
        private System.Windows.Forms.Button btnJobStart;
        private System.Windows.Forms.Button btnJobStop;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblStockCount;
        private System.Windows.Forms.Button button2;
    }
}

