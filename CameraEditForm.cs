using System;
using System.Net;
using System.Windows.Forms;

namespace PylonStream
{
    public partial class CameraEditForm : Form
    {
        public CameraConfig CameraConfig { get; private set; }

        public CameraEditForm(CameraConfig? existingConfig = null)
        {
            InitializeComponent();
            
            if (existingConfig != null)
            {
                CameraConfig = existingConfig;
                txtIp.Text = existingConfig.IpAddress;
                txtStreamName.Text = existingConfig.StreamName;
                cmbCodec.SelectedItem = existingConfig.Codec;
                cmbEncoder.SelectedItem = existingConfig.EncoderMode;
                numFps.Value = existingConfig.TargetFps;
                numPacketSize.Value = existingConfig.PacketSize;
                numInterPacketDelay.Value = existingConfig.InterPacketDelay;
                this.Text = "Edit Camera Configuration";
            }
            else
            {
                CameraConfig = new CameraConfig();
                cmbCodec.SelectedIndex = 0; // H264
                cmbEncoder.SelectedIndex = 0; // CPU
                numPacketSize.Value = 1500;
                numInterPacketDelay.Value = 1000;
                this.Text = "Add New Camera";
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            string ip = txtIp.Text.Trim();
            string streamName = txtStreamName.Text.Trim();

            // Validate IP
            if (!IPAddress.TryParse(ip, out _))
            {
                MessageBox.Show("Please enter a valid IP address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIp.Focus();
                return;
            }

            // Validate Stream Path
            if (string.IsNullOrEmpty(streamName))
            {
                MessageBox.Show("RTSP Stream Path cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStreamName.Focus();
                return;
            }

            // Populate config properties
            CameraConfig.IpAddress = ip;
            CameraConfig.StreamName = streamName;
            CameraConfig.Codec = cmbCodec.SelectedItem?.ToString() ?? "H264";
            CameraConfig.EncoderMode = cmbEncoder.SelectedItem?.ToString() ?? "CPU";
            CameraConfig.TargetFps = (int)numFps.Value;
            CameraConfig.PacketSize = (int)numPacketSize.Value;
            CameraConfig.InterPacketDelay = (int)numInterPacketDelay.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
