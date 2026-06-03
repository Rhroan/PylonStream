using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PylonStream.Services;

namespace PylonStream
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Subscribe to system log events
            Modules.SystemLogger.OnLogAdded += SystemLogger_OnLogAdded;

            // Load existing logs
            var initialLogs = Modules.SystemLogger.GetRecentLogs();
            foreach (var log in initialLogs)
            {
                AppendLogToUI(log);
            }

            // Initialize Grid structures
            RebuildCameraGrid();

            // Start status UI refresh timer
            tmrRefresh.Start();

            Modules.SystemLogger.AddLog("Application UI Dashboard initialized.", "INFO");
        }

        private string GetLocalIPAddress()
        {
            return Modules.GetServerIP();
        }

        private void RebuildCameraGrid()
        {
            dgvCameras.Rows.Clear();
            string localIp = GetLocalIPAddress();
            foreach (var cam in Modules.Config.Cameras)
            {
                string rtspUrl = $"rtsp://{localIp}:{Modules.Config.MediamtxPort}/{cam.StreamName}";
                string encoderInfo = $"{cam.Codec} ({cam.EncoderMode})";
                
                dgvCameras.Rows.Add(
                    cam.IpAddress,
                    rtspUrl,
                    encoderInfo,
                    $"{cam.TargetFps} / 0.0 FPS",
                    "Stopped"
                );
            }
        }

        private void SystemLogger_OnLogAdded(LogEntry log)
        {
            if (this.IsDisposed) return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<LogEntry>(SystemLogger_OnLogAdded), log);
            }
            else
            {
                AppendLogToUI(log);
            }
        }

        private void AppendLogToUI(LogEntry log)
        {
            if (rtbLogs.IsDisposed) return;

            // Keep log length in check
            if (rtbLogs.TextLength > 50000)
            {
                rtbLogs.Select(0, 10000);
                rtbLogs.SelectedText = "";
            }

            rtbLogs.SelectionStart = rtbLogs.TextLength;
            rtbLogs.SelectionLength = 0;

            // Time prefix
            rtbLogs.SelectionColor = Color.FromArgb(100, 116, 139); // Slate Gray
            rtbLogs.AppendText($"[{log.Time}] ");

            // Level indicator
            Color levelColor = Color.FromArgb(56, 189, 248); // Blue
            if (log.Level == "WARN") levelColor = Color.FromArgb(245, 158, 11); // Orange
            if (log.Level == "ERROR") levelColor = Color.FromArgb(248, 113, 113); // Red

            rtbLogs.SelectionColor = levelColor;
            rtbLogs.SelectionFont = new Font(rtbLogs.Font, FontStyle.Bold);
            rtbLogs.AppendText($"[{log.Level}] ");

            // Message text
            rtbLogs.SelectionColor = Color.FromArgb(226, 232, 240); // Off-White
            rtbLogs.SelectionFont = new Font(rtbLogs.Font, FontStyle.Regular);
            rtbLogs.AppendText($"{log.Message}{Environment.NewLine}");

            // Auto scroll to bottom
            rtbLogs.ScrollToCaret();
        }

        private void TmrRefresh_Tick(object sender, EventArgs e)
        {
            // 1. Refresh MediaMTX Server status indicator
            bool mtxRunning = Modules.MediaMTXMgr.IsRunning;
            if (mtxRunning)
            {
                lblMtxStatusVal.Text = "Running (Port " + Modules.Config.MediamtxPort + ")";
                lblMtxStatusVal.BackColor = Color.FromArgb(220, 252, 231); // Light Green
                lblMtxStatusVal.ForeColor = Color.FromArgb(21, 128, 61); // Dark Green
            }
            else
            {
                lblMtxStatusVal.Text = "Stopped";
                lblMtxStatusVal.BackColor = Color.FromArgb(254, 240, 138); // Light Yellow
                lblMtxStatusVal.ForeColor = Color.FromArgb(133, 77, 14); // Dark Yellow
            }

            // 2. Refresh Cameras status grid
            var pipelines = Modules.CameraMgr.GetPipelines();

            // If config has been edited and camera count mismatch, rebuild
            if (dgvCameras.RowCount != Modules.Config.Cameras.Count)
            {
                RebuildCameraGrid();
            }

            for (int i = 0; i < dgvCameras.RowCount; i++)
            {
                string ip = dgvCameras.Rows[i].Cells[0].Value.ToString() ?? "";
                var pipeline = pipelines.FirstOrDefault(p => p.IpAddress == ip);

                if (pipeline != null)
                {
                    dgvCameras.Rows[i].Cells[3].Value = $"{pipeline.TargetFps} / {pipeline.CurrentFps:F1} FPS";
                    dgvCameras.Rows[i].Cells[4].Value = pipeline.Status.ToString();
                }
                else
                {
                    dgvCameras.Rows[i].Cells[3].Value = "0 / 0.0 FPS";
                    dgvCameras.Rows[i].Cells[4].Value = "Stopped";
                }
            }
        }

        private void BtnStartAll_Click(object sender, EventArgs e)
        {
            btnStartAll.Enabled = false;
            Modules.SystemLogger.AddLog("User triggered START all services.", "INFO");

            // Start MediaMTX in background
            Modules.MediaMTXMgr.Start();

            // Start all cameras
            Modules.CameraMgr.StartAll();

            btnStopAll.Enabled = true;
        }

        private void BtnStopAll_Click(object sender, EventArgs e)
        {
            btnStopAll.Enabled = false;
            Modules.SystemLogger.AddLog("User triggered STOP all services.", "INFO");

            // Stop all cameras
            Modules.CameraMgr.StopAll();

            // Stop MediaMTX
            Modules.MediaMTXMgr.Stop();

            btnStartAll.Enabled = true;
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            bool wasRunning = Modules.CameraMgr.IsRunning;

            using (var settings = new SettingsForm())
            {
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    RebuildCameraGrid();
                    
                    if (wasRunning)
                    {
                        var result = MessageBox.Show(
                            "Configuration updated. Do you want to restart the streaming services now to apply settings?",
                            "Restart Service",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            BtnStopAll_Click(this, EventArgs.Empty);
                            BtnStartAll_Click(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrRefresh.Stop();
            Modules.SystemLogger.OnLogAdded -= SystemLogger_OnLogAdded;
        }
    }
}
