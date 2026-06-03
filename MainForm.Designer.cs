namespace PylonStream
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpControl = new System.Windows.Forms.GroupBox();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.lblMtxStatusVal = new System.Windows.Forms.Label();
            this.lblMtxStatus = new System.Windows.Forms.Label();
            this.grpCameras = new System.Windows.Forms.GroupBox();
            this.dgvCameras = new System.Windows.Forms.DataGridView();
            this.colIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRtsp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEncoder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpLogs = new System.Windows.Forms.GroupBox();
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.tmrRefresh = new System.Windows.Forms.Timer();
            this.grpControl.SuspendLayout();
            this.grpCameras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).BeginInit();
            this.grpLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpControl
            // 
            this.grpControl.Controls.Add(this.btnSettings);
            this.grpControl.Controls.Add(this.btnStopAll);
            this.grpControl.Controls.Add(this.btnStartAll);
            this.grpControl.Controls.Add(this.lblMtxStatusVal);
            this.grpControl.Controls.Add(this.lblMtxStatus);
            this.grpControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpControl.Location = new System.Drawing.Point(12, 12);
            this.grpControl.Name = "grpControl";
            this.grpControl.Size = new System.Drawing.Size(960, 70);
            this.grpControl.TabIndex = 0;
            this.grpControl.TabStop = false;
            this.grpControl.Text = "Streaming Service Control Panel";
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(825, 23);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(120, 32);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings...";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnStopAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopAll.ForeColor = System.Drawing.Color.White;
            this.btnStopAll.Location = new System.Drawing.Point(690, 23);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(120, 32);
            this.btnStopAll.TabIndex = 3;
            this.btnStopAll.Text = "Stop Service";
            this.btnStopAll.UseVisualStyleBackColor = false;
            this.btnStopAll.Click += new System.EventHandler(this.BtnStopAll_Click);
            // 
            // btnStartAll
            // 
            this.btnStartAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnStartAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartAll.ForeColor = System.Drawing.Color.White;
            this.btnStartAll.Location = new System.Drawing.Point(525, 23);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(150, 32);
            this.btnStartAll.TabIndex = 2;
            this.btnStartAll.Text = "Start Stream Service";
            this.btnStartAll.UseVisualStyleBackColor = false;
            this.btnStartAll.Click += new System.EventHandler(this.BtnStartAll_Click);
            // 
            // lblMtxStatusVal
            // 
            this.lblMtxStatusVal.AutoSize = true;
            this.lblMtxStatusVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(240)))), ((int)(((byte)(138)))));
            this.lblMtxStatusVal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMtxStatusVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(77)))), ((int)(((byte)(14)))));
            this.lblMtxStatusVal.Location = new System.Drawing.Point(170, 29);
            this.lblMtxStatusVal.Name = "lblMtxStatusVal";
            this.lblMtxStatusVal.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.lblMtxStatusVal.Size = new System.Drawing.Size(78, 23);
            this.lblMtxStatusVal.TabIndex = 1;
            this.lblMtxStatusVal.Text = "Stopped";
            // 
            // lblMtxStatus
            // 
            this.lblMtxStatus.AutoSize = true;
            this.lblMtxStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMtxStatus.Location = new System.Drawing.Point(20, 32);
            this.lblMtxStatus.Name = "lblMtxStatus";
            this.lblMtxStatus.Size = new System.Drawing.Size(139, 17);
            this.lblMtxStatus.TabIndex = 0;
            this.lblMtxStatus.Text = "MediaMTX RTSP Server:";
            // 
            // grpCameras
            // 
            this.grpCameras.Controls.Add(this.dgvCameras);
            this.grpCameras.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpCameras.Location = new System.Drawing.Point(12, 95);
            this.grpCameras.Name = "grpCameras";
            this.grpCameras.Size = new System.Drawing.Size(960, 350);
            this.grpCameras.TabIndex = 1;
            this.grpCameras.TabStop = false;
            this.grpCameras.Text = "Active Cameras & Streams Status";
            // 
            // dgvCameras
            // 
            this.dgvCameras.AllowUserToAddRows = false;
            this.dgvCameras.AllowUserToDeleteRows = false;
            this.dgvCameras.AllowUserToResizeRows = false;
            this.dgvCameras.BackgroundColor = System.Drawing.Color.White;
            this.dgvCameras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCameras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIp,
            this.colRtsp,
            this.colEncoder,
            this.colFps,
            this.colStatus});
            this.dgvCameras.Location = new System.Drawing.Point(15, 25);
            this.dgvCameras.MultiSelect = false;
            this.dgvCameras.Name = "dgvCameras";
            this.dgvCameras.ReadOnly = true;
            this.dgvCameras.RowHeadersVisible = false;
            this.dgvCameras.RowTemplate.Height = 28;
            this.dgvCameras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCameras.Size = new System.Drawing.Size(930, 310);
            this.dgvCameras.TabIndex = 0;
            // 
            // colIp
            // 
            this.colIp.HeaderText = "Camera IP";
            this.colIp.Name = "colIp";
            this.colIp.ReadOnly = true;
            this.colIp.Width = 140;
            // 
            // colRtsp
            // 
            this.colRtsp.HeaderText = "RTSP Stream Path";
            this.colRtsp.Name = "colRtsp";
            this.colRtsp.ReadOnly = true;
            this.colRtsp.Width = 330;
            // 
            // colEncoder
            // 
            this.colEncoder.HeaderText = "Encoder Configuration";
            this.colEncoder.Name = "colEncoder";
            this.colEncoder.ReadOnly = true;
            this.colEncoder.Width = 200;
            // 
            // colFps
            // 
            this.colFps.HeaderText = "Target / Current FPS";
            this.colFps.Name = "colFps";
            this.colFps.ReadOnly = true;
            this.colFps.Width = 130;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 110;
            // 
            // grpLogs
            // 
            this.grpLogs.Controls.Add(this.rtbLogs);
            this.grpLogs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpLogs.Location = new System.Drawing.Point(12, 455);
            this.grpLogs.Name = "grpLogs";
            this.grpLogs.Size = new System.Drawing.Size(960, 220);
            this.grpLogs.TabIndex = 2;
            this.grpLogs.TabStop = false;
            this.grpLogs.Text = "System Console Logs";
            // 
            // rtbLogs
            // 
            this.rtbLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.rtbLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLogs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtbLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.rtbLogs.Location = new System.Drawing.Point(15, 25);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.ReadOnly = true;
            this.rtbLogs.Size = new System.Drawing.Size(930, 180);
            this.rtbLogs.TabIndex = 0;
            this.rtbLogs.Text = "";
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Interval = 500;
            this.tmrRefresh.Tick += new System.EventHandler(this.TmrRefresh_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 691);
            this.Controls.Add(this.grpLogs);
            this.Controls.Add(this.grpCameras);
            this.Controls.Add(this.grpControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Basler GigE Camera RTSP Streamer Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpControl.ResumeLayout(false);
            this.grpControl.PerformLayout();
            this.grpCameras.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).EndInit();
            this.grpLogs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpControl;
        private System.Windows.Forms.Label lblMtxStatus;
        private System.Windows.Forms.Label lblMtxStatusVal;
        private System.Windows.Forms.Button btnStartAll;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.GroupBox grpCameras;
        private System.Windows.Forms.DataGridView dgvCameras;
        private System.Windows.Forms.GroupBox grpLogs;
        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.Timer tmrRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRtsp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEncoder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFps;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}
