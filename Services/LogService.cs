using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace PylonStream.Services
{
    public class LogEntry
    {
        public string Time { get; set; } = string.Empty;
        public string Level { get; set; } = "INFO"; // INFO, WARN, ERROR
        public string Message { get; set; } = string.Empty;
    }

    public class LogService
    {
        private readonly object _lock = new object();
        private readonly string _logDir;
        private readonly string _prefix;
        private readonly List<LogEntry> _recentLogs = new List<LogEntry>();
        private const int MaxRecentLogs = 200;

        public event Action<LogEntry>? OnLogAdded;

        public LogService(string prefix)
        {
            _prefix = prefix;
            _logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            try
            {
                if (!Directory.Exists(_logDir))
                {
                    Directory.CreateDirectory(_logDir);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create log directory: {ex.Message}");
            }
        }

        public void AddLog(string msg, string level = "INFO")
        {
            var now = DateTime.Now;
            var timeStr = now.ToString("HH:mm:ss");
            var entry = new LogEntry
            {
                Time = timeStr,
                Level = level,
                Message = msg
            };

            lock (_lock)
            {
                // Add to memory list
                _recentLogs.Add(entry);
                if (_recentLogs.Count > MaxRecentLogs)
                {
                    _recentLogs.RemoveAt(0);
                }

                // Write to file
                try
                {
                    string dateStr = now.ToString("yyyyMMdd");
                    string logFile = Path.Combine(_logDir, $"{dateStr}_{_prefix}.log");
                    string logLine = $"[{now:yyyy-MM-dd HH:mm:ss}] [{level}] {msg}{Environment.NewLine}";
                    File.AppendAllText(logFile, logLine, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing log file: {ex.Message}");
                }
            }

            // Raise local event (for WinForms UI)
            try
            {
                OnLogAdded?.Invoke(entry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error raising log event: {ex.Message}");
            }

            // Push to SignalR Web Clients
            PushToWebClients(entry);
        }

        private async void PushToWebClients(LogEntry entry)
        {
            try
            {
                // We use dynamic hub push
                if (Modules.WebHubContext != null)
                {
                    await Modules.WebHubContext.Clients.All.SendAsync("ReceiveLog", entry.Time, entry.Level, entry.Message);
                }
            }
            catch
            {
                // Ignore background SignalR transmission errors
            }
        }

        public List<LogEntry> GetRecentLogs()
        {
            lock (_lock)
            {
                return new List<LogEntry>(_recentLogs);
            }
        }
    }
}
