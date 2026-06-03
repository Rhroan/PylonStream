using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Basler.Pylon;

namespace PylonStream
{
    public enum PipelineStatus
    {
        Stopped,
        Init,
        Connected,
        Grabbing,
        Reconnecting,
        Error
    }

    public class CameraStreamPipeline
    {
        private readonly CameraConfig _config;
        private Camera? _camera;
        private Process? _ffmpegProcess;
        private CancellationTokenSource? _cts;
        private Task? _pipelineTask;
        
        private readonly object _statusLock = new object();
        private PipelineStatus _status = PipelineStatus.Stopped;
        
        private int _width = 0;
        private int _height = 0;
        private double _currentFps = 0.0;

        public string IpAddress => _config.IpAddress;
        public string StreamName => _config.StreamName;
        public string Codec => _config.Codec;
        public string EncoderMode => _config.EncoderMode;
        public int TargetFps => _config.TargetFps;

        public PipelineStatus Status
        {
            get { lock (_statusLock) return _status; }
            private set { lock (_statusLock) _status = value; }
        }

        public double CurrentFps => _currentFps;
        public int Width => _width;
        public int Height => _height;

        public CameraStreamPipeline(CameraConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            lock (_statusLock)
            {
                if (Status != PipelineStatus.Stopped)
                {
                    return;
                }
                Status = PipelineStatus.Init;
                _cts = new CancellationTokenSource();
                _pipelineTask = Task.Run(() => PipelineLoop(_cts.Token));
                Modules.SystemLogger.AddLog($"Pipeline for camera {IpAddress} started.", "INFO");
            }
        }

        public void Stop()
        {
            lock (_statusLock)
            {
                if (Status == PipelineStatus.Stopped)
                {
                    return;
                }

                Modules.SystemLogger.AddLog($"Stopping pipeline for camera {IpAddress}...", "INFO");
                _cts?.Cancel();
                
                try
                {
                    // Wait for task to finish (with timeout)
                    _pipelineTask?.Wait(2000);
                }
                catch
                {
                    // Ignore task wait exceptions
                }

                CleanupPipeline();
                _cts?.Dispose();
                _cts = null;
                _pipelineTask = null;
                
                _currentFps = 0.0;
                Status = PipelineStatus.Stopped;
                Modules.SystemLogger.AddLog($"Pipeline for camera {IpAddress} stopped.", "INFO");
            }
        }

        private void PipelineLoop(CancellationToken token)
        {
            var converter = new PixelDataConverter();
            byte[]? frameBuffer = null;
            bool isColor = false;

            int targetIntervalMs = 1000 / Math.Max(1, _config.TargetFps);
            var fpsStopwatch = Stopwatch.StartNew();
            int frameCount = 0;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (Status == PipelineStatus.Init || Status == PipelineStatus.Reconnecting)
                    {
                        CleanupPipeline();
                        
                        Modules.SystemLogger.AddLog($"Searching for camera with IP: {IpAddress}...", "INFO");
                        
                        // Try to find the camera by IP Address
                        ICameraInfo? targetDeviceInfo = null;
                        foreach (var device in CameraFinder.Enumerate())
                        {
                            if (device.ContainsKey(CameraInfoKey.DeviceIpAddress))
                            {
                                string ip = device[CameraInfoKey.DeviceIpAddress];
                                if (ip == IpAddress)
                                {
                                    targetDeviceInfo = device;
                                    break;
                                }
                            }
                        }

                        if (targetDeviceInfo == null)
                        {
                            Status = PipelineStatus.Reconnecting;
                            Modules.SystemLogger.AddLog($"Camera {IpAddress} not found. Retrying in 5 seconds...", "WARN");
                            SleepOrCancel(5000, token);
                            continue;
                        }

                        // Instantiate camera
                        _camera = new Camera(targetDeviceInfo);
                        _camera.Open();
                        Status = PipelineStatus.Connected;
                        Modules.SystemLogger.AddLog($"Camera {IpAddress} connected successfully.", "INFO");

                        // Determine if color or mono
                        string pixelFormat = _camera.Parameters[PLCamera.PixelFormat].GetValue();
                        isColor = !(pixelFormat.StartsWith("Mono", StringComparison.OrdinalIgnoreCase));
                        Modules.SystemLogger.AddLog($"Camera {IpAddress} format: {pixelFormat} (Color: {isColor})", "INFO");

                        // Optional: Try to set camera frame rate
                        try
                        {
                            if (_camera.Parameters.Contains(PLCamera.AcquisitionFrameRateEnable))
                            {
                                _camera.Parameters[PLCamera.AcquisitionFrameRateEnable].SetValue(true);
                            }
                            if (_camera.Parameters.Contains(PLCamera.AcquisitionFrameRate))
                            {
                                _camera.Parameters[PLCamera.AcquisitionFrameRate].SetValue((double)_config.TargetFps);
                            }
                        }
                        catch (Exception ex)
                        {
                            Modules.SystemLogger.AddLog($"Warning: Could not set frame rate parameters on camera {IpAddress}: {ex.Message}", "WARN");
                        }

                        // Try to optimize GigE network transmission settings to prevent dropouts (Incomplete Grabs)
                        try
                        {
                            // 1. Packet Size (MTU) - Set from config (default 1500, can be 9000 if Jumbo Frames are supported)
                            if (_camera.Parameters.Contains(PLCamera.GevSCPSPacketSize))
                            {
                                int packetSize = _config.PacketSize > 0 ? _config.PacketSize : 1500;
                                _camera.Parameters[PLCamera.GevSCPSPacketSize].SetValue(packetSize);
                                Modules.SystemLogger.AddLog($"Camera {IpAddress} GevSCPSPacketSize set to {packetSize}.", "INFO");
                            }

                            // 2. Inter-Packet Delay - Add padding between ethernet packets to resolve collisions
                            if (_camera.Parameters.Contains(PLCamera.GevSCPD))
                            {
                                int delay = _config.InterPacketDelay >= 0 ? _config.InterPacketDelay : 1000;
                                _camera.Parameters[PLCamera.GevSCPD].SetValue(delay);
                                Modules.SystemLogger.AddLog($"Camera {IpAddress} GevSCPD (Inter-Packet Delay) set to {delay}.", "INFO");
                            }

                            // 3. Heartbeat Timeout - Set to 2000ms so the camera releases its lock quickly if the app crashes
                            if (_camera.Parameters.Contains(PLCamera.GevHeartbeatTimeout))
                            {
                                _camera.Parameters[PLCamera.GevHeartbeatTimeout].SetValue(2000);
                                Modules.SystemLogger.AddLog($"Camera {IpAddress} GevHeartbeatTimeout set to 2000ms.", "INFO");
                            }
                        }
                        catch (Exception ex)
                        {
                            Modules.SystemLogger.AddLog($"Warning: Could not optimize GigE network parameters for camera {IpAddress}: {ex.Message}", "WARN");
                        }

                        // Retrieve resolution
                        _width = (int)_camera.Parameters[PLCamera.Width].GetValue();
                        _height = (int)_camera.Parameters[PLCamera.Height].GetValue();

                        // Configure pixel converter
                        converter.OutputPixelFormat = isColor ? PixelType.RGB8packed : PixelType.Mono8;
                        int bytesPerPixel = isColor ? 3 : 1;
                        frameBuffer = new byte[_width * _height * bytesPerPixel];

                        // Start FFmpeg process
                        StartFFmpeg(isColor);

                        // Start FFmpeg process
                        StartFFmpeg(isColor);

                        // Increase driver's buffer pool size to cushion processing delays
                        try
                        {
                            if (_camera.Parameters.Contains(PLStream.MaxNumBuffer))
                            {
                                _camera.Parameters[PLStream.MaxNumBuffer].SetValue(64);
                                Modules.SystemLogger.AddLog($"Camera {IpAddress} Parameters PLStream.MaxNumBuffer set to 64.", "INFO");
                            }
                        }
                        catch (Exception ex)
                        {
                            Modules.SystemLogger.AddLog($"Warning: Could not set Parameters PLStream.MaxNumBuffer for camera {IpAddress}: {ex.Message}", "WARN");
                        }

                        // Start grabbing infinitely using standard latest images strategy
                        _camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByUser);
                        Status = PipelineStatus.Grabbing;
                        Modules.SystemLogger.AddLog($"Camera {IpAddress} streaming started. Resolution: {_width}x{_height}", "INFO");

                        fpsStopwatch.Restart();
                        frameCount = 0;
                    }

                    if (Status == PipelineStatus.Grabbing && _camera != null && _ffmpegProcess != null && frameBuffer != null)
                    {
                        if (!_camera.StreamGrabber.IsGrabbing)
                        {
                            throw new Exception("Stream grabber is no longer grabbing.");
                        }

                        var loopStopwatch = Stopwatch.StartNew();

                        // Retrieve result
                        using (IGrabResult grabResult = _camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException))
                        {
                            if (grabResult == null)
                            {
                                throw new Exception("RetrieveResult returned null (grabber may have stopped).");
                            }

                            if (grabResult.GrabSucceeded)
                            {
                                // Convert frame
                                converter.Convert(frameBuffer, grabResult);

                                // Write to FFmpeg stdin
                                if (_ffmpegProcess.StandardInput.BaseStream.CanWrite)
                                {
                                    _ffmpegProcess.StandardInput.BaseStream.Write(frameBuffer, 0, frameBuffer.Length);
                                    _ffmpegProcess.StandardInput.BaseStream.Flush();
                                }
                                else
                                {
                                    throw new IOException("FFmpeg pipe is not writable.");
                                }

                                frameCount++;
                            }
                            else
                            {
                                Modules.SystemLogger.AddLog($"Grab failed on camera {IpAddress}: {grabResult.ErrorCode} - {grabResult.ErrorDescription}", "WARN");
                            }
                        }

                        // Calculate FPS
                        if (fpsStopwatch.ElapsedMilliseconds >= 1000)
                        {
                            _currentFps = Math.Round((double)frameCount * 1000.0 / fpsStopwatch.ElapsedMilliseconds, 1);
                            frameCount = 0;
                            fpsStopwatch.Restart();
                        }

                        // Software throttle to match target FPS (if camera generates frames faster)
                        loopStopwatch.Stop();
                        int elapsed = (int)loopStopwatch.ElapsedMilliseconds;
                        int sleepTime = targetIntervalMs - elapsed;
                        if (sleepTime > 0)
                        {
                            SleepOrCancel(sleepTime, token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _currentFps = 0.0;
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    Status = PipelineStatus.Reconnecting;
                    Modules.SystemLogger.AddLog($"Exception in pipeline loop for {IpAddress}: {ex.Message}. Entering reconnecting mode.", "ERROR");
                    CleanupPipeline();
                    SleepOrCancel(5000, token);
                }
            }

            CleanupPipeline();
        }

        private void StartFFmpeg(bool isColor)
        {
            string ffmpegExe = Modules.Config.FfmpegPath;
            if (!Path.IsPathRooted(ffmpegExe))
            {
                ffmpegExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegExe);
            }

            if (!File.Exists(ffmpegExe))
            {
                throw new FileNotFoundException($"FFmpeg executable not found at: {ffmpegExe}");
            }

            string inputPixFmt = isColor ? "rgb24" : "gray";
            string codecArg = "";
            string encoderParams = "";

            if (Codec.Equals("H265", StringComparison.OrdinalIgnoreCase) || Codec.Equals("HEVC", StringComparison.OrdinalIgnoreCase))
            {
                if (EncoderMode.Equals("NVENC", StringComparison.OrdinalIgnoreCase))
                {
                    codecArg = "-c:v hevc_nvenc";
                    encoderParams = $"-preset p1 -g {TargetFps} -bf 0";
                }
                else if (EncoderMode.Equals("QSV", StringComparison.OrdinalIgnoreCase))
                {
                    codecArg = "-c:v hevc_qsv";
                    encoderParams = $"-preset veryfast -g {TargetFps} -bf 0";
                }
                else
                {
                    codecArg = "-c:v libx265";
                    encoderParams = $"-preset ultrafast -g {TargetFps} -bf 0";
                }
            }
            else // Default to H264
            {
                if (EncoderMode.Equals("NVENC", StringComparison.OrdinalIgnoreCase))
                {
                    codecArg = "-c:v h264_nvenc";
                    encoderParams = $"-preset p1 -g {TargetFps} -bf 0";
                }
                else if (EncoderMode.Equals("QSV", StringComparison.OrdinalIgnoreCase))
                {
                    codecArg = "-c:v h264_qsv";
                    encoderParams = $"-preset veryfast -g {TargetFps} -bf 0";
                }
                else
                {
                    codecArg = "-c:v libx264";
                    encoderParams = $"-preset ultrafast -g {TargetFps} -bf 0";
                }
            }

            // Target RTSP path
            string pushHost = Modules.Config.SelectedInterfaceIp == "Auto" ? "127.0.0.1" : Modules.Config.SelectedInterfaceIp;
            string rtspUrl = $"rtsp://{pushHost}:{Modules.Config.MediamtxPort}/{StreamName}";

            // Arguments syntax:
            // -f rawvideo -pix_fmt rgb24 -s 1920x1080 -r 30 -i pipe:0 [codec settings] -pix_fmt yuv420p -f rtsp -rtsp_transport tcp rtsp://url
            string args = $"-f rawvideo -pix_fmt {inputPixFmt} -s {_width}x{_height} -r {TargetFps} -i pipe:0 " +
                          $"{codecArg} {encoderParams} -pix_fmt yuv420p -f rtsp -rtsp_transport tcp {rtspUrl}";

            Modules.SystemLogger.AddLog($"Starting FFmpeg: {ffmpegExe} {args}", "INFO");

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegExe,
                Arguments = args,
                WorkingDirectory = Path.GetDirectoryName(ffmpegExe),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true // Redirect error stream to catch encoder startup messages
            };

            _ffmpegProcess = new Process { StartInfo = startInfo };
            
            // Log FFmpeg errors/status
            _ffmpegProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    // FFmpeg logs to stderr by default. We only want to log it if it looks like an error, or just filter it.
                    if (e.Data.Contains("Error", StringComparison.OrdinalIgnoreCase) || e.Data.Contains("Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        Modules.SystemLogger.AddLog($"FFmpeg [{IpAddress}]: {e.Data.Trim()}", "WARN");
                    }
                }
            };

            _ffmpegProcess.Start();
            _ffmpegProcess.BeginErrorReadLine();
        }

        private void SleepOrCancel(int milliseconds, CancellationToken token)
        {
            try
            {
                Task.Delay(milliseconds, token).Wait(token);
            }
            catch
            {
                // Ignore delay cancellation exceptions
            }
        }

        private void CleanupPipeline()
        {
            try
            {
                if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
                {
                    _ffmpegProcess.Kill(true);
                    Modules.SystemLogger.AddLog($"FFmpeg process for camera {IpAddress} terminated.", "INFO");
                }
            }
            catch {}
            finally
            {
                _ffmpegProcess?.Dispose();
                _ffmpegProcess = null;
            }

            try
            {
                if (_camera != null)
                {
                    if (_camera.StreamGrabber.IsGrabbing)
                    {
                        _camera.StreamGrabber.Stop();
                    }
                    _camera.Close();
                    _camera.Dispose();
                    Modules.SystemLogger.AddLog($"Basler Camera {IpAddress} closed and disposed.", "INFO");
                }
            }
            catch {}
            finally
            {
                _camera = null;
            }
        }
    }
}
