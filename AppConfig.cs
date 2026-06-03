using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PylonStream
{
    public class CameraConfig
    {
        public string IpAddress { get; set; } = "192.168.1.100";
        public string StreamName { get; set; } = "cam1";
        public string Codec { get; set; } = "H264"; // H264 or H265
        public string EncoderMode { get; set; } = "CPU"; // CPU, NVENC, QSV
        public int TargetFps { get; set; } = 30;
        public int Width { get; set; } = 0; // 0 means use camera default
        public int Height { get; set; } = 0; // 0 means use camera default
        public int PacketSize { get; set; } = 1500; // Default to 1500 (standard Ethernet)
        public int InterPacketDelay { get; set; } = 1000; // Delay in ticks
    }

    public class AppConfig
    {
        public string FfmpegPath { get; set; } = Path.Combine("Binaries", "ffmpeg.exe");
        public string MediamtxPath { get; set; } = Path.Combine("Binaries", "mediamtx.exe");
        public int MediamtxPort { get; set; } = 8554;
        public int WebApiPort { get; set; } = 5000;
        public string SelectedInterfaceIp { get; set; } = "Auto";
        public List<CameraConfig> Cameras { get; set; } = new List<CameraConfig>();

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static AppConfig Load()
        {
            lock (Modules.ConfigLock)
            {
                try
                {
                    if (File.Exists(ConfigPath))
                    {
                        string json = File.ReadAllText(ConfigPath);
                        var config = JsonSerializer.Deserialize<AppConfig>(json);
                        if (config != null)
                        {
                            return config;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading config: {ex.Message}");
                }

                // Return default config if loading fails or file doesn't exist
                var defaultConfig = new AppConfig();
                // Add a default camera entry to make it easier for users
                defaultConfig.Cameras.Add(new CameraConfig());
                return defaultConfig;
            }
        }

        public void Save()
        {
            lock (Modules.ConfigLock)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(this, options);
                    File.WriteAllText(ConfigPath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving config: {ex.Message}");
                }
            }
        }

        public AppConfig Clone()
        {
            lock (Modules.ConfigLock)
            {
                var copy = new AppConfig
                {
                    FfmpegPath = this.FfmpegPath,
                    MediamtxPath = this.MediamtxPath,
                    MediamtxPort = this.MediamtxPort,
                    WebApiPort = this.WebApiPort,
                    SelectedInterfaceIp = this.SelectedInterfaceIp,
                    Cameras = new List<CameraConfig>()
                };

                foreach (var cam in this.Cameras)
                {
                    copy.Cameras.Add(new CameraConfig
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

                return copy;
            }
        }
    }
}
