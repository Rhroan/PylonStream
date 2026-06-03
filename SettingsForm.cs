using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PylonStream
{
    public partial class SettingsForm : Form
    {
        private List<CameraConfig> _localCameras = new List<CameraConfig>();
        private BindingSource _bindingSource = new BindingSource();

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Populate Global Settings
            txtFfmpeg.Text = Modules.Config.FfmpegPath;
            txtMediamtx.Text = Modules.Config.MediamtxPath;
            numMtxPort.Value = Modules.Config.MediamtxPort;
            numApiPort.Value = Modules.Config.WebApiPort;

            // Populate Network Interfaces
            cmbNetworkInterface.Items.Clear();
            cmbNetworkInterface.Items.Add("Auto (Default Socket Routing)");
            
            string currentSelected = Modules.Config.SelectedInterfaceIp;
            int selectedIdx = 0;

            try
            {
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                    {
                        var ipProps = ni.GetIPProperties();
                        foreach (var addr in ipProps.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                string ipStr = addr.Address.ToString();
                                string itemText = $"{ipStr} ({ni.Description})";
                                int addedIdx = cmbNetworkInterface.Items.Add(itemText);
                                if (ipStr == currentSelected)
                                {
                                    selectedIdx = addedIdx;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Modules.SystemLogger.AddLog($"Error listing network interfaces for settings UI: {ex.Message}", "WARN");
            }

            cmbNetworkInterface.SelectedIndex = selectedIdx;

            // Clone camera list to avoid changing global settings prematurely
            foreach (var cam in Modules.Config.Cameras)
            {
                _localCameras.Add(new CameraConfig
                {
                    IpAddress = cam.IpAddress,
                    StreamName = cam.StreamName,
                    Codec = cam.Codec,
                    EncoderMode = cam.EncoderMode,
                    TargetFps = cam.TargetFps,
                    Width = cam.Width,
                    Height = cam.Height,
                    PacketSize = cam.PacketSize,
                    InterPacketDelay = cam.InterPacketDelay
                });
            }

            _bindingSource.DataSource = _localCameras;
            dgvCameras.AutoGenerateColumns = false;
            dgvCameras.DataSource = _bindingSource;
        }

        private void BtnBrowseFfmpeg_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                ofd.Title = "Select ffmpeg.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFfmpeg.Text = ofd.FileName;
                }
            }
        }

        private void BtnBrowseMediamtx_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                ofd.Title = "Select mediamtx.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtMediamtx.Text = ofd.FileName;
                }
            }
        }

        private void BtnAddCam_Click(object sender, EventArgs e)
        {
            using (var editForm = new CameraEditForm())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _bindingSource.Add(editForm.CameraConfig);
                    _bindingSource.ResetBindings(false);
                }
            }
        }

        private void BtnEditCam_Click(object sender, EventArgs e)
        {
            if (dgvCameras.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a camera row to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedConfig = dgvCameras.SelectedRows[0].DataBoundItem as CameraConfig;
            if (selectedConfig != null)
            {
                // Create a clone for editing
                var tempConfig = new CameraConfig
                {
                    IpAddress = selectedConfig.IpAddress,
                    StreamName = selectedConfig.StreamName,
                    Codec = selectedConfig.Codec,
                    EncoderMode = selectedConfig.EncoderMode,
                    TargetFps = selectedConfig.TargetFps,
                    Width = selectedConfig.Width,
                    Height = selectedConfig.Height,
                    PacketSize = selectedConfig.PacketSize,
                    InterPacketDelay = selectedConfig.InterPacketDelay
                };

                using (var editForm = new CameraEditForm(tempConfig))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Copy values back
                        selectedConfig.IpAddress = tempConfig.IpAddress;
                        selectedConfig.StreamName = tempConfig.StreamName;
                        selectedConfig.Codec = tempConfig.Codec;
                        selectedConfig.EncoderMode = tempConfig.EncoderMode;
                        selectedConfig.TargetFps = tempConfig.TargetFps;
                        selectedConfig.PacketSize = tempConfig.PacketSize;
                        selectedConfig.InterPacketDelay = tempConfig.InterPacketDelay;

                        _bindingSource.ResetBindings(false);
                    }
                }
            }
        }

        private void BtnDeleteCam_Click(object sender, EventArgs e)
        {
            if (dgvCameras.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a camera row to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedConfig = dgvCameras.SelectedRows[0].DataBoundItem as CameraConfig;
            if (selectedConfig != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete camera {selectedConfig.IpAddress}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _bindingSource.Remove(selectedConfig);
                    _bindingSource.ResetBindings(false);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string ffmpeg = txtFfmpeg.Text.Trim();
            string mediamtx = txtMediamtx.Text.Trim();

            if (string.IsNullOrEmpty(ffmpeg) || string.IsNullOrEmpty(mediamtx))
            {
                MessageBox.Show("Paths for FFmpeg and MediaMTX cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save back to Modules.Config
            Modules.Config.FfmpegPath = ffmpeg;
            Modules.Config.MediamtxPath = mediamtx;
            Modules.Config.MediamtxPort = (int)numMtxPort.Value;
            Modules.Config.WebApiPort = (int)numApiPort.Value;

            if (cmbNetworkInterface.SelectedIndex <= 0)
            {
                Modules.Config.SelectedInterfaceIp = "Auto";
            }
            else
            {
                string? selectedItem = cmbNetworkInterface.SelectedItem?.ToString();
                if (selectedItem != null)
                {
                    int spaceIndex = selectedItem.IndexOf(' ');
                    if (spaceIndex > 0)
                    {
                        Modules.Config.SelectedInterfaceIp = selectedItem.Substring(0, spaceIndex);
                    }
                    else
                    {
                        Modules.Config.SelectedInterfaceIp = selectedItem;
                    }
                }
                else
                {
                    Modules.Config.SelectedInterfaceIp = "Auto";
                }
            }

            Modules.Config.Cameras.Clear();
            foreach (var cam in _localCameras)
            {
                Modules.Config.Cameras.Add(cam);
            }

            Modules.Config.Save();
            Modules.SystemLogger.AddLog("System configurations saved successfully.", "INFO");

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
