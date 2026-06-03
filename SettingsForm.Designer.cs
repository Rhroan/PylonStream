namespace PylonStream
{
    partial class SettingsForm
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
            this.grpGlobal = new System.Windows.Forms.GroupBox();
            this.lblFfmpeg = new System.Windows.Forms.Label();
            this.txtFfmpeg = new System.Windows.Forms.TextBox();
            this.btnBrowseFfmpeg = new System.Windows.Forms.Button();
            this.lblMediamtx = new System.Windows.Forms.Label();
            this.txtMediamtx = new System.Windows.Forms.TextBox();
            this.btnBrowseMediamtx = new System.Windows.Forms.Button();
            this.lblMtxPort = new System.Windows.Forms.Label();
            this.numMtxPort = new System.Windows.Forms.NumericUpDown();
            this.lblApiPort = new System.Windows.Forms.Label();
            this.numApiPort = new System.Windows.Forms.NumericUpDown();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cmbNetworkInterface = new System.Windows.Forms.ComboBox();
            this.grpCameras = new System.Windows.Forms.GroupBox();
            this.dgvCameras = new System.Windows.Forms.DataGridView();
            this.colIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEncoder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddCam = new System.Windows.Forms.Button();
            this.btnEditCam = new System.Windows.Forms.Button();
            this.btnDeleteCam = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpGlobal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMtxPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiPort)).BeginInit();
            this.grpCameras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).BeginInit();
            this.SuspendLayout();
            // 
            // grpGlobal
            // 
            this.grpGlobal.Controls.Add(this.cmbNetworkInterface);
            this.grpGlobal.Controls.Add(this.lblInterface);
            this.grpGlobal.Controls.Add(this.numApiPort);
            this.grpGlobal.Controls.Add(this.lblApiPort);
            this.grpGlobal.Controls.Add(this.numMtxPort);
            this.grpGlobal.Controls.Add(this.lblMtxPort);
            this.grpGlobal.Controls.Add(this.btnBrowseMediamtx);
            this.grpGlobal.Controls.Add(this.txtMediamtx);
            this.grpGlobal.Controls.Add(this.lblMediamtx);
            this.grpGlobal.Controls.Add(this.btnBrowseFfmpeg);
            this.grpGlobal.Controls.Add(this.txtFfmpeg);
            this.grpGlobal.Controls.Add(this.lblFfmpeg);
            this.grpGlobal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpGlobal.Location = new System.Drawing.Point(12, 12);
            this.grpGlobal.Name = "grpGlobal";
            this.grpGlobal.Size = new System.Drawing.Size(600, 180);
            this.grpGlobal.TabIndex = 0;
            this.grpGlobal.TabStop = false;
            this.grpGlobal.Text = "Global Paths & Ports Configuration";
            // 
            // lblFfmpeg
            // 
            this.lblFfmpeg.AutoSize = true;
            this.lblFfmpeg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFfmpeg.Location = new System.Drawing.Point(15, 30);
            this.lblFfmpeg.Name = "lblFfmpeg";
            this.lblFfmpeg.Size = new System.Drawing.Size(107, 15);
            this.lblFfmpeg.TabIndex = 0;
            this.lblFfmpeg.Text = "FFmpeg Executable:";
            // 
            // txtFfmpeg
            // 
            this.txtFfmpeg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtFfmpeg.Location = new System.Drawing.Point(140, 27);
            this.txtFfmpeg.Name = "txtFfmpeg";
            this.txtFfmpeg.Size = new System.Drawing.Size(360, 23);
            this.txtFfmpeg.TabIndex = 1;
            // 
            // btnBrowseFfmpeg
            // 
            this.btnBrowseFfmpeg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBrowseFfmpeg.Location = new System.Drawing.Point(510, 26);
            this.btnBrowseFfmpeg.Name = "btnBrowseFfmpeg";
            this.btnBrowseFfmpeg.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseFfmpeg.TabIndex = 2;
            this.btnBrowseFfmpeg.Text = "Browse...";
            this.btnBrowseFfmpeg.UseVisualStyleBackColor = true;
            this.btnBrowseFfmpeg.Click += new System.EventHandler(this.BtnBrowseFfmpeg_Click);
            // 
            // lblMediamtx
            // 
            this.lblMediamtx.AutoSize = true;
            this.lblMediamtx.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMediamtx.Location = new System.Drawing.Point(15, 65);
            this.lblMediamtx.Name = "lblMediamtx";
            this.lblMediamtx.Size = new System.Drawing.Size(120, 15);
            this.lblMediamtx.TabIndex = 3;
            this.lblMediamtx.Text = "MediaMTX Executable:";
            // 
            // txtMediamtx
            // 
            this.txtMediamtx.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMediamtx.Location = new System.Drawing.Point(140, 62);
            this.txtMediamtx.Name = "txtMediamtx";
            this.txtMediamtx.Size = new System.Drawing.Size(360, 23);
            this.txtMediamtx.TabIndex = 4;
            // 
            // btnBrowseMediamtx
            // 
            this.btnBrowseMediamtx.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBrowseMediamtx.Location = new System.Drawing.Point(510, 61);
            this.btnBrowseMediamtx.Name = "btnBrowseMediamtx";
            this.btnBrowseMediamtx.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseMediamtx.TabIndex = 5;
            this.btnBrowseMediamtx.Text = "Browse...";
            this.btnBrowseMediamtx.UseVisualStyleBackColor = true;
            this.btnBrowseMediamtx.Click += new System.EventHandler(this.BtnBrowseMediamtx_Click);
            // 
            // lblMtxPort
            // 
            this.lblMtxPort.AutoSize = true;
            this.lblMtxPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMtxPort.Location = new System.Drawing.Point(15, 105);
            this.lblMtxPort.Name = "lblMtxPort";
            this.lblMtxPort.Size = new System.Drawing.Size(89, 15);
            this.lblMtxPort.TabIndex = 6;
            this.lblMtxPort.Text = "RTSP Server Port:";
            // 
            // numMtxPort
            // 
            this.numMtxPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numMtxPort.Location = new System.Drawing.Point(140, 103);
            this.numMtxPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numMtxPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMtxPort.Name = "numMtxPort";
            this.numMtxPort.Size = new System.Drawing.Size(120, 23);
            this.numMtxPort.TabIndex = 7;
            this.numMtxPort.Value = new decimal(new int[] {
            8554,
            0,
            0,
            0});
            // 
            // lblApiPort
            // 
            this.lblApiPort.AutoSize = true;
            this.lblApiPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblApiPort.Location = new System.Drawing.Point(315, 105);
            this.lblApiPort.Name = "lblApiPort";
            this.lblApiPort.Size = new System.Drawing.Size(82, 15);
            this.lblApiPort.TabIndex = 8;
            this.lblApiPort.Text = "Web API Port:";
            // 
            // numApiPort
            // 
            this.numApiPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numApiPort.Location = new System.Drawing.Point(415, 103);
            this.numApiPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numApiPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numApiPort.Name = "numApiPort";
            this.numApiPort.Size = new System.Drawing.Size(120, 23);
            this.numApiPort.TabIndex = 9;
            this.numApiPort.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInterface.Location = new System.Drawing.Point(15, 145);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(107, 15);
            this.lblInterface.TabIndex = 10;
            this.lblInterface.Text = "Network Interface:";
            // 
            // cmbNetworkInterface
            // 
            this.cmbNetworkInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNetworkInterface.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbNetworkInterface.FormattingEnabled = true;
            this.cmbNetworkInterface.Location = new System.Drawing.Point(140, 142);
            this.cmbNetworkInterface.Name = "cmbNetworkInterface";
            this.cmbNetworkInterface.Size = new System.Drawing.Size(360, 23);
            this.cmbNetworkInterface.TabIndex = 11;
            // 
            // grpCameras
            // 
            this.grpCameras.Controls.Add(this.btnDeleteCam);
            this.grpCameras.Controls.Add(this.btnEditCam);
            this.grpCameras.Controls.Add(this.btnAddCam);
            this.grpCameras.Controls.Add(this.dgvCameras);
            this.grpCameras.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.grpCameras.Location = new System.Drawing.Point(12, 205);
            this.grpCameras.Name = "grpCameras";
            this.grpCameras.Size = new System.Drawing.Size(600, 260);
            this.grpCameras.TabIndex = 1;
            this.grpCameras.TabStop = false;
            this.grpCameras.Text = "Camera Connection Map";
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
            this.colPath,
            this.colCodec,
            this.colEncoder,
            this.colFps});
            this.dgvCameras.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCameras.Location = new System.Drawing.Point(15, 25);
            this.dgvCameras.MultiSelect = false;
            this.dgvCameras.Name = "dgvCameras";
            this.dgvCameras.RowHeadersVisible = false;
            this.dgvCameras.RowTemplate.Height = 25;
            this.dgvCameras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCameras.Size = new System.Drawing.Size(570, 185);
            this.dgvCameras.TabIndex = 0;
            // 
            // colIp
            // 
            this.colIp.DataPropertyName = "IpAddress";
            this.colIp.HeaderText = "Camera IP Address";
            this.colIp.Name = "colIp";
            this.colIp.Width = 130;
            // 
            // colPath
            // 
            this.colPath.DataPropertyName = "StreamName";
            this.colPath.HeaderText = "RTSP Stream Path";
            this.colPath.Name = "colPath";
            this.colPath.Width = 140;
            // 
            // colCodec
            // 
            this.colCodec.DataPropertyName = "Codec";
            this.colCodec.HeaderText = "Codec";
            this.colCodec.Name = "colCodec";
            this.colCodec.Width = 80;
            // 
            // colEncoder
            // 
            this.colEncoder.DataPropertyName = "EncoderMode";
            this.colEncoder.HeaderText = "Encoder Mode";
            this.colEncoder.Name = "colEncoder";
            this.colEncoder.Width = 120;
            // 
            // colFps
            // 
            this.colFps.DataPropertyName = "TargetFps";
            this.colFps.HeaderText = "Target FPS";
            this.colFps.Name = "colFps";
            this.colFps.Width = 90;
            // 
            // btnAddCam
            // 
            this.btnAddCam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnAddCam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAddCam.ForeColor = System.Drawing.Color.White;
            this.btnAddCam.Location = new System.Drawing.Point(295, 220);
            this.btnAddCam.Name = "btnAddCam";
            this.btnAddCam.Size = new System.Drawing.Size(90, 28);
            this.btnAddCam.TabIndex = 1;
            this.btnAddCam.Text = "Add Camera";
            this.btnAddCam.UseVisualStyleBackColor = false;
            this.btnAddCam.Click += new System.EventHandler(this.BtnAddCam_Click);
            // 
            // btnEditCam
            // 
            this.btnEditCam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnEditCam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditCam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnEditCam.ForeColor = System.Drawing.Color.White;
            this.btnEditCam.Location = new System.Drawing.Point(395, 220);
            this.btnEditCam.Name = "btnEditCam";
            this.btnEditCam.Size = new System.Drawing.Size(90, 28);
            this.btnEditCam.TabIndex = 2;
            this.btnEditCam.Text = "Edit Selected";
            this.btnEditCam.UseVisualStyleBackColor = false;
            this.btnEditCam.Click += new System.EventHandler(this.BtnEditCam_Click);
            // 
            // btnDeleteCam
            // 
            this.btnDeleteCam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDeleteCam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDeleteCam.ForeColor = System.Drawing.Color.White;
            this.btnDeleteCam.Location = new System.Drawing.Point(495, 220);
            this.btnDeleteCam.Name = "btnDeleteCam";
            this.btnDeleteCam.Size = new System.Drawing.Size(90, 28);
            this.btnDeleteCam.TabIndex = 3;
            this.btnDeleteCam.Text = "Delete Selected";
            this.btnDeleteCam.UseVisualStyleBackColor = false;
            this.btnDeleteCam.Click += new System.EventHandler(this.BtnDeleteCam_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(340, 480);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(160, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save Configuration";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(510, 480);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(624, 531);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpCameras);
            this.Controls.Add(this.grpGlobal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Streaming & Camera Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.grpGlobal.ResumeLayout(false);
            this.grpGlobal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMtxPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiPort)).EndInit();
            this.grpCameras.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpGlobal;
        private System.Windows.Forms.Label lblFfmpeg;
        private System.Windows.Forms.TextBox txtFfmpeg;
        private System.Windows.Forms.Button btnBrowseFfmpeg;
        private System.Windows.Forms.Label lblMediamtx;
        private System.Windows.Forms.TextBox txtMediamtx;
        private System.Windows.Forms.Button btnBrowseMediamtx;
        private System.Windows.Forms.Label lblMtxPort;
        private System.Windows.Forms.NumericUpDown numMtxPort;
        private System.Windows.Forms.Label lblApiPort;
        private System.Windows.Forms.NumericUpDown numApiPort;
        private System.Windows.Forms.GroupBox grpCameras;
        private System.Windows.Forms.DataGridView dgvCameras;
        private System.Windows.Forms.Button btnAddCam;
        private System.Windows.Forms.Button btnEditCam;
        private System.Windows.Forms.Button btnDeleteCam;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodec;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEncoder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFps;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cmbNetworkInterface;
    }
}
