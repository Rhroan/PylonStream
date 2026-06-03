namespace PylonStream
{
    partial class CameraEditForm
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
            this.lblIp = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.lblStreamName = new System.Windows.Forms.Label();
            this.txtStreamName = new System.Windows.Forms.TextBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cmbCodec = new System.Windows.Forms.ComboBox();
            this.lblEncoder = new System.Windows.Forms.Label();
            this.cmbEncoder = new System.Windows.Forms.ComboBox();
            this.lblFps = new System.Windows.Forms.Label();
            this.numFps = new System.Windows.Forms.NumericUpDown();
            this.lblPacketSize = new System.Windows.Forms.Label();
            this.numPacketSize = new System.Windows.Forms.NumericUpDown();
            this.lblInterPacketDelay = new System.Windows.Forms.Label();
            this.numInterPacketDelay = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPacketSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterPacketDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblIp.Location = new System.Drawing.Point(20, 25);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(95, 15);
            this.lblIp.TabIndex = 0;
            this.lblIp.Text = "Camera IP Address:";
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtIp.Location = new System.Drawing.Point(140, 22);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(180, 23);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "192.168.1.100";
            // 
            // lblStreamName
            // 
            this.lblStreamName.AutoSize = true;
            this.lblStreamName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStreamName.Location = new System.Drawing.Point(20, 65);
            this.lblStreamName.Name = "lblStreamName";
            this.lblStreamName.Size = new System.Drawing.Size(107, 15);
            this.lblStreamName.TabIndex = 2;
            this.lblStreamName.Text = "RTSP Stream Path:";
            // 
            // txtStreamName
            // 
            this.txtStreamName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtStreamName.Location = new System.Drawing.Point(140, 62);
            this.txtStreamName.Name = "txtStreamName";
            this.txtStreamName.Size = new System.Drawing.Size(180, 23);
            this.txtStreamName.TabIndex = 3;
            this.txtStreamName.Text = "cam1";
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCodec.Location = new System.Drawing.Point(20, 105);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(81, 15);
            this.lblCodec.TabIndex = 4;
            this.lblCodec.Text = "Video Codec:";
            // 
            // cmbCodec
            // 
            this.cmbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCodec.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbCodec.FormattingEnabled = true;
            this.cmbCodec.Items.AddRange(new object[] {
            "H264",
            "H265"});
            this.cmbCodec.Location = new System.Drawing.Point(140, 102);
            this.cmbCodec.Name = "cmbCodec";
            this.cmbCodec.Size = new System.Drawing.Size(180, 23);
            this.cmbCodec.TabIndex = 5;
            // 
            // lblEncoder
            // 
            this.lblEncoder.AutoSize = true;
            this.lblEncoder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblEncoder.Location = new System.Drawing.Point(20, 145);
            this.lblEncoder.Name = "lblEncoder";
            this.lblEncoder.Size = new System.Drawing.Size(87, 15);
            this.lblEncoder.TabIndex = 6;
            this.lblEncoder.Text = "Encoder Mode:";
            // 
            // cmbEncoder
            // 
            this.cmbEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbEncoder.FormattingEnabled = true;
            this.cmbEncoder.Items.AddRange(new object[] {
            "CPU",
            "NVENC",
            "QSV"});
            this.cmbEncoder.Location = new System.Drawing.Point(140, 142);
            this.cmbEncoder.Name = "cmbEncoder";
            this.cmbEncoder.Size = new System.Drawing.Size(180, 23);
            this.cmbEncoder.TabIndex = 7;
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFps.Location = new System.Drawing.Point(20, 185);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(65, 15);
            this.lblFps.TabIndex = 8;
            this.lblFps.Text = "Target FPS:";
            // 
            // numFps
            // 
            this.numFps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numFps.Location = new System.Drawing.Point(140, 183);
            this.numFps.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numFps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFps.Name = "numFps";
            this.numFps.Size = new System.Drawing.Size(180, 23);
            this.numFps.TabIndex = 9;
            this.numFps.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Location = new System.Drawing.Point(140, 310);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 30);
            this.btnOk.TabIndex = 14;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(235, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // lblPacketSize
            // 
            this.lblPacketSize.AutoSize = true;
            this.lblPacketSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPacketSize.Location = new System.Drawing.Point(20, 225);
            this.lblPacketSize.Name = "lblPacketSize";
            this.lblPacketSize.Size = new System.Drawing.Size(104, 15);
            this.lblPacketSize.TabIndex = 10;
            this.lblPacketSize.Text = "Packet Size (MTU):";
            // 
            // numPacketSize
            // 
            this.numPacketSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numPacketSize.Location = new System.Drawing.Point(140, 223);
            this.numPacketSize.Maximum = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.numPacketSize.Minimum = new decimal(new int[] {
            576,
            0,
            0,
            0});
            this.numPacketSize.Name = "numPacketSize";
            this.numPacketSize.Size = new System.Drawing.Size(180, 23);
            this.numPacketSize.TabIndex = 11;
            this.numPacketSize.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // lblInterPacketDelay
            // 
            this.lblInterPacketDelay.AutoSize = true;
            this.lblInterPacketDelay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInterPacketDelay.Location = new System.Drawing.Point(20, 265);
            this.lblInterPacketDelay.Name = "lblInterPacketDelay";
            this.lblInterPacketDelay.Size = new System.Drawing.Size(105, 15);
            this.lblInterPacketDelay.TabIndex = 12;
            this.lblInterPacketDelay.Text = "Inter-Packet Delay:";
            // 
            // numInterPacketDelay
            // 
            this.numInterPacketDelay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numInterPacketDelay.Location = new System.Drawing.Point(140, 263);
            this.numInterPacketDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numInterPacketDelay.Name = "numInterPacketDelay";
            this.numInterPacketDelay.Size = new System.Drawing.Size(180, 23);
            this.numInterPacketDelay.TabIndex = 13;
            this.numInterPacketDelay.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // CameraEditForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 360);
            this.Controls.Add(this.numInterPacketDelay);
            this.Controls.Add(this.lblInterPacketDelay);
            this.Controls.Add(this.numPacketSize);
            this.Controls.Add(this.lblPacketSize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.numFps);
            this.Controls.Add(this.lblFps);
            this.Controls.Add(this.cmbEncoder);
            this.Controls.Add(this.lblEncoder);
            this.Controls.Add(this.cmbCodec);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.txtStreamName);
            this.Controls.Add(this.lblStreamName);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.lblIp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CameraEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add / Edit Camera";
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPacketSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInterPacketDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblStreamName;
        private System.Windows.Forms.TextBox txtStreamName;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cmbCodec;
        private System.Windows.Forms.Label lblEncoder;
        private System.Windows.Forms.ComboBox cmbEncoder;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.NumericUpDown numFps;
        private System.Windows.Forms.Label lblPacketSize;
        private System.Windows.Forms.NumericUpDown numPacketSize;
        private System.Windows.Forms.Label lblInterPacketDelay;
        private System.Windows.Forms.NumericUpDown numInterPacketDelay;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
