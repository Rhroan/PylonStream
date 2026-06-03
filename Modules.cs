using Microsoft.AspNetCore.SignalR;
using PylonStream.Services;
using PylonStream.Hubs;

namespace PylonStream
{
    public static class Modules
    {
        public static AppConfig Config { get; set; } = new AppConfig();
        public static LogService SystemLogger { get; set; } = new LogService("System");
        public static CameraManager CameraMgr { get; set; } = new CameraManager();
        public static MediaMTXManager MediaMTXMgr { get; set; } = new MediaMTXManager();
        public static IHubContext<StreamHub>? WebHubContext { get; set; }
        public static Microsoft.AspNetCore.Builder.WebApplication? WebApp { get; set; }
        public static readonly object ConfigLock = new object();

        public static string GetServerIP()
        {
            lock (ConfigLock)
            {
                if (!string.IsNullOrEmpty(Config.SelectedInterfaceIp) && Config.SelectedInterfaceIp != "Auto")
                {
                    return Config.SelectedInterfaceIp;
                }
            }

            try
            {
                using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    var endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                    if (endPoint != null)
                    {
                        return endPoint.Address.ToString();
                    }
                }
            }
            catch
            {
                try
                {
                    var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.ToString();
                        }
                    }
                }
                catch { }
            }
            return "127.0.0.1";
        }
    }
}
