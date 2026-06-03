using System;
using System.Collections.Generic;
using System.Linq;

namespace PylonStream
{
    public class CameraManager
    {
        private readonly List<CameraStreamPipeline> _pipelines = new List<CameraStreamPipeline>();
        private readonly object _lock = new object();
        private bool _isRunning = false;

        public bool IsRunning => _isRunning;

        public void StartAll()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    Modules.SystemLogger.AddLog("Camera service is already running.", "INFO");
                    return;
                }

                Modules.SystemLogger.AddLog("Starting all camera stream pipelines...", "INFO");
                
                // Clear any existing pipelines
                StopAllInternal();

                // Create and start a pipeline for each configured camera
                foreach (var camConfig in Modules.Config.Cameras)
                {
                    try
                    {
                        var pipeline = new CameraStreamPipeline(camConfig);
                        _pipelines.Add(pipeline);
                        pipeline.Start();
                    }
                    catch (Exception ex)
                    {
                        Modules.SystemLogger.AddLog($"Failed to initialize pipeline for camera {camConfig.IpAddress}: {ex.Message}", "ERROR");
                    }
                }

                _isRunning = true;
                Modules.SystemLogger.AddLog("All camera stream pipelines initialization completed.", "INFO");
            }
        }

        public void StopAll()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    return;
                }

                Modules.SystemLogger.AddLog("Stopping all camera stream pipelines...", "INFO");
                StopAllInternal();
                _isRunning = false;
                Modules.SystemLogger.AddLog("All camera stream pipelines stopped.", "INFO");
            }
        }

        private void StopAllInternal()
        {
            foreach (var pipeline in _pipelines)
            {
                try
                {
                    pipeline.Stop();
                }
                catch (Exception ex)
                {
                    Modules.SystemLogger.AddLog($"Error stopping pipeline for camera {pipeline.IpAddress}: {ex.Message}", "WARN");
                }
            }
            _pipelines.Clear();
        }

        public List<CameraStreamPipeline> GetPipelines()
        {
            lock (_lock)
            {
                return _pipelines.ToList(); // Return a copy of the list
            }
        }

        public bool RestartPipeline(string ip)
        {
            lock (_lock)
            {
                var pipeline = _pipelines.FirstOrDefault(p => p.IpAddress == ip);
                if (pipeline != null)
                {
                    Modules.SystemLogger.AddLog($"Manually restarting pipeline for camera {ip}...", "INFO");
                    pipeline.Stop();
                    pipeline.Start();
                    return true;
                }
                return false;
            }
        }
    }
}
