using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PylonStream.Services;

namespace PylonStream.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        // GET /api/status
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            try
            {
                var pipelines = Modules.CameraMgr.GetPipelines();
                // Safe clone config to avoid collection modified exception during multi-thread access
                var configSnapshot = Modules.Config.Clone();
                var cameraStatuses = configSnapshot.Cameras.Select(cam => new
                {
                    ipAddress = cam.IpAddress,
                    streamName = cam.StreamName,
                    status = pipelineExists(pipelines, cam.IpAddress) ? getPipelineStatus(pipelines, cam.IpAddress) : "Stopped",
                    fps = pipelineExists(pipelines, cam.IpAddress) ? getPipelineFps(pipelines, cam.IpAddress) : 0.0,
                    width = pipelineExists(pipelines, cam.IpAddress) ? getPipelineWidth(pipelines, cam.IpAddress) : 0,
                    height = pipelineExists(pipelines, cam.IpAddress) ? getPipelineHeight(pipelines, cam.IpAddress) : 0,
                    codec = cam.Codec,
                    encoder = cam.EncoderMode,
                    targetFps = cam.TargetFps,
                    packetSize = cam.PacketSize,
                    interPacketDelay = cam.InterPacketDelay
                }).ToList();

                var status = new
                {
                    serverRunning = Modules.CameraMgr.IsRunning,
                    mediamtxRunning = Modules.MediaMTXMgr.IsRunning,
                    mediamtxPort = configSnapshot.MediamtxPort,
                    selectedInterfaceIp = configSnapshot.SelectedInterfaceIp,
                    ffmpegPath = configSnapshot.FfmpegPath,
                    mediamtxPath = configSnapshot.MediamtxPath,
                    cameras = cameraStatuses
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Helper methods to avoid complex expressions inside Linq Select
        private bool pipelineExists(List<CameraStreamPipeline> pipelines, string ip) => pipelines.Any(p => p.IpAddress == ip);
        private string getPipelineStatus(List<CameraStreamPipeline> pipelines, string ip) => pipelines.First(p => p.IpAddress == ip).Status.ToString();
        private double getPipelineFps(List<CameraStreamPipeline> pipelines, string ip) => pipelines.First(p => p.IpAddress == ip).CurrentFps;
        private int getPipelineWidth(List<CameraStreamPipeline> pipelines, string ip) => pipelines.First(p => p.IpAddress == ip).Width;
        private int getPipelineHeight(List<CameraStreamPipeline> pipelines, string ip) => pipelines.First(p => p.IpAddress == ip).Height;

        // GET /api/interfaces
        [HttpGet("interfaces")]
        public IActionResult GetInterfaces()
        {
            try
            {
                var interfaces = new List<object>();
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
                                interfaces.Add(new
                                {
                                    name = ni.Name,
                                    description = ni.Description,
                                    ipAddress = addr.Address.ToString()
                                });
                            }
                        }
                    }
                }
                return Ok(interfaces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET /api/config
        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            try
            {
                return Ok(Modules.Config.Clone());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        // GET /api/logs
        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            try
            {
                var logs = Modules.SystemLogger.GetRecentLogs();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/control/start
        [HttpPost("control/start")]
        public IActionResult StartService()
        {
            try
            {
                Task.Run(() =>
                {
                    // Start MediaMTX
                    Modules.MediaMTXMgr.Start();
                    // Start Cameras
                    Modules.CameraMgr.StartAll();
                });

                return Ok(new { message = "Service start request triggered." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/control/stop
        [HttpPost("control/stop")]
        public IActionResult StopService()
        {
            try
            {
                Task.Run(() =>
                {
                    // Stop Cameras
                    Modules.CameraMgr.StopAll();
                    // Stop MediaMTX
                    Modules.MediaMTXMgr.Stop();
                });

                return Ok(new { message = "Service stop request triggered." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/control/restart-camera?ip=192.168.1.101
        [HttpPost("control/restart-camera")]
        public IActionResult RestartCamera([FromQuery] string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return BadRequest(new { error = "IP address query parameter is required." });
            }

            try
            {
                bool success = Modules.CameraMgr.RestartPipeline(ip);
                if (success)
                {
                    return Ok(new { message = $"Restart request for camera {ip} triggered." });
                }
                else
                {
                    return NotFound(new { error = $"Camera with IP {ip} not found in configuration." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/config
        [HttpPost("config")]
        public IActionResult UpdateConfig([FromBody] AppConfig newConfig)
        {
            if (newConfig == null)
            {
                return BadRequest(new { error = "Invalid configuration data." });
            }

            try
            {
                lock (Modules.ConfigLock)
                {
                    Modules.Config = newConfig;
                    Modules.Config.Save();
                }
                Modules.SystemLogger.AddLog("Configuration updated via REST API.", "INFO");
                return Ok(new { message = "Configuration saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
